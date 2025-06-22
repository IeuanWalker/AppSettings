using System.Collections.Immutable;
using System.Text;
using IeuanWalker.AppSettings.Generator.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace IeuanWalker.AppSettings.Generator;

[Generator]
public class AppSettingsSourceGenerator : IIncrementalGenerator
{
	const string fullInterfaceBase = "IeuanWalker.AppSettings.IAppSettings";
	const string fullInterface = "IeuanWalker.AppSettings.IAppSettings`1";
	static string? assemblyName;

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		// Find any type declarations with base lists (potential interface implementers)
		IncrementalValuesProvider<TypeDeclarationSyntax?> typeDeclarations = context.SyntaxProvider
			.CreateSyntaxProvider(
				predicate: static (s, _) => s is TypeDeclarationSyntax { BaseList: not null },
				transform: static (ctx, _) => ctx.Node as TypeDeclarationSyntax)
			.Where(type => type != null);

		// Combine with compilation for semantic analysis
		IncrementalValueProvider<(Compilation, ImmutableArray<TypeDeclarationSyntax?>)> compilationAndTypes
			= context.CompilationProvider.Combine(typeDeclarations.Collect());

		// Generate source output
		context.RegisterSourceOutput(compilationAndTypes,
			(spc, source) => Execute(source.Item1, source.Item2, spc));
	}

	static void Execute(Compilation compilation, ImmutableArray<TypeDeclarationSyntax?> types, SourceProductionContext context)
	{
		// Get the assembly name from the compilation
		assemblyName = compilation.Assembly.Name.Trim();

		List<(string SettingsClassFullName, string? ValidatorClassFullName)> settingsClasses = [];

		// Get the IAppSettings interface symbol to check against
		INamedTypeSymbol? appSettingsInterfaceBase = compilation.GetTypeByMetadataName(fullInterfaceBase);

		if (appSettingsInterfaceBase is null)
		{
			return;
		}

		INamedTypeSymbol? appSettingsInterface = compilation.GetTypeByMetadataName(fullInterface);

		if (appSettingsInterface is null)
		{
			return;
		}

		foreach (TypeDeclarationSyntax? typeDeclaration in types)
		{
			if (typeDeclaration is null)
			{
				continue;
			}

			SemanticModel semanticModel = compilation.GetSemanticModel(typeDeclaration.SyntaxTree);
			INamedTypeSymbol? typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration);

			if (typeSymbol is null || typeSymbol.IsAbstract)
			{
				continue;
			}

			// Check if the type implements IAppSettings<T>
			foreach (INamedTypeSymbol interfaceType in typeSymbol.AllInterfaces)
			{


				if (SymbolEqualityComparer.Default.Equals(interfaceType.OriginalDefinition, appSettingsInterface) && interfaceType.TypeArguments.Length == 1)
				{
					ITypeSymbol validatorType = interfaceType.TypeArguments[0];

					// Check if validator is actually for this type
					bool isValidValidator = false;

					// Check if it's a named type that we can examine
					if (validatorType is INamedTypeSymbol namedValidatorType)
					{
						// Check base class (AbstractValidator<T>)
						if (namedValidatorType.BaseType != null)
						{
							if (namedValidatorType.BaseType.IsGenericType &&
								namedValidatorType.BaseType.TypeArguments.Length == 1 &&
								SymbolEqualityComparer.Default.Equals(namedValidatorType.BaseType.TypeArguments[0], typeSymbol))
							{
								isValidValidator = true;
							}
						}

						// Check interfaces (IValidator<T>)
						foreach (var iface in namedValidatorType.AllInterfaces)
						{
							if (iface.IsGenericType &&
								iface.Name == "IValidator" &&
								iface.TypeArguments.Length == 1 &&
								SymbolEqualityComparer.Default.Equals(iface.TypeArguments[0], typeSymbol))
							{
								isValidValidator = true;
								break;
							}
						}
					}

					if (isValidValidator)
					{
						settingsClasses.Add((typeSymbol.ToDisplayString(), validatorType.ToDisplayString()));
					}
					else
					{
						// Report diagnostic that validator is not for the correct type
						context.ReportDiagnostic(Diagnostic.Create(
							new DiagnosticDescriptor(
								id: "APPSET001",
								title: "Invalid validator type",
								messageFormat: "The validator type '{0}' must validate the settings class '{1}'",
								category: "AppSettings",
								defaultSeverity: DiagnosticSeverity.Error,
								isEnabledByDefault: true),
							typeDeclaration.GetLocation(),
							validatorType.Name,
							typeSymbol.Name));
					}
					break;
				}
				else if (SymbolEqualityComparer.Default.Equals(interfaceType.OriginalDefinition, appSettingsInterfaceBase))
				{
					settingsClasses.Add((typeSymbol.ToDisplayString(), null));
					break;
				}
			}
		}

		if (settingsClasses.Count == 0)
		{
			return;
		}

		// Check if IHostApplicationBuilder is available
		bool hasIHostApplicationBuilder = compilation.GetTypeByMetadataName("Microsoft.Extensions.Hosting.IHostApplicationBuilder") is not null;
		bool hasMauiAppBuilder = compilation.GetTypeByMetadataName("Microsoft.Maui.Hosting.MauiAppBuilder") is not null;

		string source = GenerateAppSettingsConfigurationClass(settingsClasses, hasIHostApplicationBuilder, hasMauiAppBuilder);
		context.AddSource("AppSettingsConfiguration.g.cs", SourceText.From(source, Encoding.UTF8));
	}

	static string GenerateAppSettingsConfigurationClass(List<(string SettingsClassName, string? ValidatorClassName)> settingsClasses, bool hasIHostApplicationBuilder, bool hasMauiAppBuilder)
	{
		StringBuilder builder = new();

		builder.Append(@"// ---------------
// <auto-generated>
//   Generated by the IeuanWalker.AppSettings
//   https://github.com/IeuanWalker/IeuanWalker.AppSettings
// </auto-generated>
// ---------------

using IeuanWalker.AppSettings;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace ").Append(assemblyName).Append(@";

public static class AppSettingsConfiguration
{
	public static Microsoft.Extensions.DependencyInjection.IServiceCollection AddAppSettingsFrom").Append(assemblyName?.Sanitize(string.Empty) ?? "Assembly").Append(@"(this Microsoft.Extensions.DependencyInjection.IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
	{
");
		foreach ((string settingsClass, string? validatorClass) in settingsClasses)
		{
			if (validatorClass is null)
			{
				builder.AppendLine($"\t\tservices.AddOptions<global::{settingsClass}>().Configure(options => configuration.GetSection(global::{settingsClass}.SectionName ?? typeof(global::{settingsClass}).Name).Bind(options));");
				builder.AppendLine();
			}
			else
			{
				builder.AppendLine($"\t\tservices.AddScoped<global::IValidator<global::{settingsClass}>, global::{validatorClass}>();");
				builder.AppendLine($"\t\tservices.AddOptions<global::{settingsClass}>()");
				builder.AppendLine($"\t\t\t.Configure(options => configuration.GetSection(global::{settingsClass}.SectionName ?? typeof(global::{settingsClass}).Name).Bind(options))");
				builder.AppendLine($"\t\t\t.ValidateFluentValidation()");
				builder.AppendLine($"\t\t\t.ValidateOnStart();");
				builder.AppendLine();
			}
		}

		builder.Append(@"  
		return services;
	}");

		// Only add the IHostApplicationBuilder extension if it's available
		if (hasIHostApplicationBuilder)
		{
			builder.Append(@"
    public static Microsoft.Extensions.Hosting.IHostApplicationBuilder AddAppSettingsFrom").Append(assemblyName?.Sanitize(string.Empty) ?? "Assembly").Append(@"(this Microsoft.Extensions.Hosting.IHostApplicationBuilder builder)
    {
        builder.Services.AddAppSettingsFrom").Append(assemblyName?.Sanitize(string.Empty) ?? "Assembly").Append(@"(builder.Configuration);
        
        return builder;
    }");
		}

		// Only add the MauiAppBuilder extension if it's available
		if (hasMauiAppBuilder)
		{
			builder.Append(@"
    public static MauiAppBuilder AddAppSettingsFrom").Append(assemblyName?.Sanitize(string.Empty) ?? "Assembly").Append(@"(this MauiAppBuilder builder)
    {
        builder.Services.AddAppSettingsFrom").Append(assemblyName?.Sanitize(string.Empty) ?? "Assembly").Append(@"(builder.Configuration);
        
        return builder;
    }");
		}

		builder.Append(@"
}");

		return builder.ToString();
	}
}