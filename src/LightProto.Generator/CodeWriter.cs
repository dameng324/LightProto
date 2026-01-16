namespace LightProto.Generator;

sealed class CodeWriter
{
    private readonly TextWriter _writer;
    private int _indent;
    private bool _atLineStart = true;

    public CodeWriter(TextWriter writer)
    {
        _writer = writer;
    }

    public IDisposable IndentScope(bool writeBraces = true, string braceEnd = "}")
    {
        return new Scope(this, writeBraces, braceEnd);
    }

    public void WriteLine(string text = "")
    {
        Write(text);
        _writer.WriteLine();
        _atLineStart = true;
    }

    public void Write(string text)
    {
        if (_atLineStart)
        {
            for (int i = 0; i < _indent; i++)
                _writer.Write("    ");
            _atLineStart = false;
        }

        _writer.Write(text);
    }

    private sealed class Scope : IDisposable
    {
        private readonly CodeWriter _writer;
        private readonly bool _writeBraces;
        private readonly string _braceEnd;

        public Scope(CodeWriter writer, bool writeBraces, string braceEnd)
        {
            _writer = writer;
            _writeBraces = writeBraces;
            _braceEnd = braceEnd;
            if (_writeBraces)
            {
                _writer.WriteLine("{");
            }
            _writer._indent++;
        }

        public void Dispose()
        {
            _writer._indent--;
            if (_writeBraces)
            {
                _writer.WriteLine(_braceEnd);
            }
        }
    }
}
