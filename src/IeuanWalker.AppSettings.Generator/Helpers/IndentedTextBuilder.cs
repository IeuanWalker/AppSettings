using System.CodeDom.Compiler;

namespace IeuanWalker.AppSettings.Generator.Helpers;

class IndentedTextBuilder : IDisposable
{
	readonly StringWriter output;
	readonly IndentedTextWriter writer;
	
	public IndentedTextBuilder()
	{
		output = new StringWriter();
		writer = new IndentedTextWriter(output);
	}

	public void Append(string value) => writer.Write(value);

	public void AppendLine(string value) => writer.WriteLine(value);

	public void AppendLine() => writer.WriteLine();

	public void IncreaseIndent() => writer.Indent++;

	public void DecreaseIndent() => writer.Indent--;

	public override string ToString() => output.ToString();
	
	public Block AppendBlock()
	{
		AppendLine("{");
		IncreaseIndent();

		return new(this);
	}
	
	public Block AppendBlock(string value)
	{
		AppendLine(value);
		return AppendBlock();
	}

	public void Dispose()
	{
		output.Dispose();
		writer.Dispose();
	}
}

struct Block(IndentedTextBuilder? builder) : IDisposable
{
	/// <summary>
	/// The <see cref="IndentedTextBuilder"/> instance to write to.
	/// </summary>
	IndentedTextBuilder? builder = builder;

	/// <inheritdoc/>
	public void Dispose()
	{
		IndentedTextBuilder? builder = this.builder;

		this.builder = null;

		if (builder is not null)
		{
			builder.DecreaseIndent();
			builder.AppendLine("}");
		}
	}
}