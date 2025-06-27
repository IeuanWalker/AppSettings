using System.CodeDom.Compiler;

namespace IeuanWalker.AppSettings.Generator.Helpers;

sealed class IndentedTextBuilder : IDisposable
{
	readonly StringWriter _output;
	readonly IndentedTextWriter _writer;

	public IndentedTextBuilder()
	{
		_output = new StringWriter();
		_writer = new IndentedTextWriter(_output);
	}

	public void Append(string value) => _writer.Write(value);

	public void AppendLine(string value) => _writer.WriteLine(value);

	public void AppendLine() => _writer.WriteLine();

	public void IncreaseIndent() => _writer.Indent++;

	public void DecreaseIndent() => _writer.Indent--;

	public override string ToString() => _output.ToString();

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
		_output.Dispose();
		_writer.Dispose();
	}
}

struct Block(IndentedTextBuilder? builder) : IDisposable
{
	IndentedTextBuilder? _builder = builder;

	public void Dispose()
	{
		IndentedTextBuilder? builder = _builder;

		_builder = null;

		if(builder is not null)
		{
			builder.DecreaseIndent();
			builder.AppendLine("}");
		}
	}
}