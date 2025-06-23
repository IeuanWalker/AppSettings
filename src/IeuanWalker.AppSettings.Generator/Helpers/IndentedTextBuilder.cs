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

	public void Write(string value) => writer.Write(value);

	public void WriteLine(string value) => writer.WriteLine(value);

	public void WriteLine() => writer.WriteLine();

	public void IncreaseIndent() => writer.Indent++;

	public void DecreaseIndent() => writer.Indent--;

	public override string ToString() => output.ToString();
	
	public Block WriteBlock()
	{
		WriteLine("{");
		IncreaseIndent();

		return new(this);
	}
	
	public Block WriteBlock(string value)
	{
		WriteLine(value);
		return WriteBlock();
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
			builder.WriteLine("}");
		}
	}
}