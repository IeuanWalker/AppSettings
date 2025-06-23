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
	const string fullAttribute = "IeuanWalker.AppSettings.SectionNameAttribute";
	static string? assemblyName;
	static INamedTypeSymbol? attributeSymbol;
	static readonly DiagnosticDescriptor diagnosticDescriptorValidatorWrongType = new(
		id: "APPSET001",
		title: "Invalid validator type",
		messageFormat: "The validator type '{0}' must validate the settings class '{1}'",
		category: "AppSettings",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

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

		List<(string SettingsClassFullName, string? ValidatorClassFullName, string sectionName)> settingsClasses = [];

		// Get the IAppSettings interface symbol to check against
		INamedTypeSymbol? appSettingsInterfaceBase = compilation.GetTypeByMetadataName(fullInterfaceBase);

		if (appSettingsInterfaceBase is null)
		{
			return;
		}

		// Get the IAppSettings`1 interface symbol to check against
		INamedTypeSymbol? appSettingsInterface = compilation.GetTypeByMetadataName(fullInterface);

		if (appSettingsInterface is null)
		{
			return;
		}

		// Get the SectionNameAttribute symbol to check against
		attributeSymbol = compilation.GetTypeByMetadataName(fullAttribute);

		// Get the IValidator`1 interface symbol to check against
		INamedTypeSymbol? iValidatorBase = compilation.GetTypeByMetadataName("FluentValidation.IValidator`1");

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

			// Check if the type implements IAppSettings or IAppSettings<T>
			foreach (INamedTypeSymbol interfaceType in typeSymbol.AllInterfaces)
			{
				// If the class doesn't implement either IAppSettings or IAppSettings<T>, skip it
				if (!(SymbolEqualityComparer.Default.Equals(interfaceType.OriginalDefinition, appSettingsInterface) && interfaceType.TypeArguments.Length == 1) &&
					!SymbolEqualityComparer.Default.Equals(interfaceType.OriginalDefinition, appSettingsInterfaceBase))
				{
					continue;
				}

				// Validate and extract the validation type, ensure the validator is for the correct type for the AppSettings class
				string? validatorClass = null;
				if (SymbolEqualityComparer.Default.Equals(interfaceType.OriginalDefinition, appSettingsInterface) && interfaceType.TypeArguments.Length == 1)
				{
					ITypeSymbol validatorType = interfaceType.TypeArguments[0];

					// Check if validator is actually for this type
					bool isValidValidator = false;

					// Check if it's a named type that we can examine
					if (validatorType is INamedTypeSymbol namedValidatorType && iValidatorBase is not null)
					{
						isValidValidator = namedValidatorType.AllInterfaces.Any(validatorInterface =>
							SymbolEqualityComparer.Default.Equals(validatorInterface.OriginalDefinition, iValidatorBase) &&
							validatorInterface.TypeArguments.Length == 1 &&
							SymbolEqualityComparer.Default.Equals(validatorInterface.TypeArguments[0], typeSymbol));
					}

					if (isValidValidator)
					{
						validatorClass = validatorType.ToDisplayString();
					}
					else
					{
						context.ReportDiagnostic(Diagnostic.Create(
							diagnosticDescriptorValidatorWrongType,
							typeDeclaration.GetLocation(),
							validatorType.Name,
							typeSymbol.Name));

						continue;
					}
				}

				settingsClasses.Add((typeSymbol.ToDisplayString(), validatorClass, GetSectionName(typeSymbol)));
			}
		}

		if (settingsClasses.Count == 0)
		{
			return;
		}

		bool hasIHostApplicationBuilder = compilation.GetTypeByMetadataName("Microsoft.Extensions.Hosting.IHostApplicationBuilder") is not null;
		bool hasMauiAppBuilder = compilation.GetTypeByMetadataName("Microsoft.Maui.Hosting.MauiAppBuilder") is not null;

		string source = GenerateAppSettingsConfigurationClass(settingsClasses, hasIHostApplicationBuilder, hasMauiAppBuilder);
		context.AddSource("AppSettingsConfiguration.g.cs", SourceText.From(source, Encoding.UTF8));
	}

	static string GetSectionName(INamedTypeSymbol namedTypeSymbol)
	{
		string sectionName = namedTypeSymbol.Name;

		if (attributeSymbol is not null)
		{
			AttributeData? sectionNameAttribute = namedTypeSymbol.GetAttributes().FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol));
			sectionName = (string?)sectionNameAttribute?.ConstructorArguments[0].Value ?? sectionName;
		}

		return sectionName;
	}

	static string GenerateAppSettingsConfigurationClass(List<(string SettingsClassName, string? ValidatorClassName, string sectionName)> settingsClasses, bool hasIHostApplicationBuilder, bool hasMauiAppBuilder)
	{
		using IndentedTextBuilder builder = new IndentedTextBuilder();
		
		string sanitisedAssemblyName = assemblyName?.Sanitize(string.Empty) ?? "Assembly";
		
		builder.WriteLine("""
		// ---------------
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
		
		""");
		
		builder.WriteLine($"namespace {assemblyName};");
		builder.WriteLine("public static class AppSettingsConfiguration");
		using (builder.WriteBlock())
		{
			builder.WriteLine($"public static Microsoft.Extensions.DependencyInjection.IServiceCollection AddAppSettingsFrom{sanitisedAssemblyName }(this Microsoft.Extensions.DependencyInjection.IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)");
			using (builder.WriteBlock())
			{
				foreach ((string settingsClass, string? validatorClass, string sectionName) in settingsClasses)
				{
					if (validatorClass is null)
					{
						builder.WriteLine($"services.AddOptions<global::{settingsClass}>()");
						builder.IncreaseIndent();
						builder.WriteLine($".Configure(options => configuration.GetSection(\"{sectionName}\").Bind(options))");
						builder.WriteLine(".ValidateDataAnnotations()");
						builder.WriteLine(".ValidateOnStart();");
					}
					else
					{
						builder.WriteLine($"services.AddScoped<IValidator<global::{settingsClass}>, global::{validatorClass}>();");
						builder.WriteLine($"services.AddOptions<global::{settingsClass}>()");
						builder.IncreaseIndent();
						builder.WriteLine($".Configure(options => configuration.GetSection(\"{sectionName}\").Bind(options))");
						builder.WriteLine($".ValidateFluentValidation()");
						builder.WriteLine($".ValidateOnStart();");
					}

					builder.DecreaseIndent();
					builder.WriteLine();
				}

				builder.WriteLine("return services;");
			}

			// Only add the IHostApplicationBuilder extension if it's available
			if (hasIHostApplicationBuilder)
			{
				builder.WriteLine();
				builder.WriteLine($"public static Microsoft.Extensions.Hosting.IHostApplicationBuilder AddAppSettingsFrom{sanitisedAssemblyName}(this Microsoft.Extensions.Hosting.IHostApplicationBuilder builder)");
				using (builder.WriteBlock())
				{
					builder.WriteLine($"builder.Services.AddAppSettingsFrom{sanitisedAssemblyName}(builder.Configuration);");
					builder.WriteLine();
					builder.WriteLine("return builder;");
				}
			}

			// Only add the MauiAppBuilder extension if it's available
			if (hasMauiAppBuilder)
			{
				builder.WriteLine($"public static MauiAppBuilder AddAppSettingsFrom{sanitisedAssemblyName}(this MauiAppBuilder builder)");
				using (builder.WriteBlock())
				{
					builder.WriteLine($"builder.Services.AddAppSettingsFrom{sanitisedAssemblyName}(this MauiAppBuilder builder);");
					builder.WriteLine();
					builder.WriteLine("return builder;");
				}
			}
		}

		return builder.ToString();
	}
}