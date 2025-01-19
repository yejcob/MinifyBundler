using System.Diagnostics;

namespace WF.MinifyBundler;

internal sealed class Compressor
{
    private const int Eof = -1;
    private int _currentChar;
    private int _nextChar;
    private int _lookahead = Eof;
    private readonly TextReader _reader;
    private readonly TextWriter _writer;

    public static string Compress(string source)
    {
        using var writer = new StringWriter();
        Compress(new StringReader(source), writer);
        return writer.ToString().Trim();
    }

    private static void Compress(TextReader reader, TextWriter writer)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(writer);

        var compressor = new Compressor(reader, writer);
        compressor.Compress();
    }

    private Compressor(TextReader reader, TextWriter writer)
    {
        Debug.Assert(reader != null);
        Debug.Assert(writer != null);

        _reader = reader;
        _writer = writer;
    }

    private void Compress()
    {
        _currentChar = '\n';
        ProcessAction(3);

        while (_currentChar != Eof)
        {
            switch (_currentChar)
            {
                case ' ':
                    ProcessAction(IsAlphanumeric(_nextChar) ? 1 : 2);
                    break;

                case '\n':
                    switch (_nextChar)
                    {
                        case '{': case '[': case '(':
                        case '+': case '-':
                            ProcessAction(1);
                            break;
                        case ' ':
                            ProcessAction(3);
                            break;
                        default:
                            ProcessAction(IsAlphanumeric(_nextChar) ? 1 : 2);
                            break;
                    }
                    break;

                default:
                    switch (_nextChar)
                    {
                        case ' ' when IsAlphanumeric(_currentChar):
                            ProcessAction(1);
                            break;
                        case '\n':
                            ProcessNewlineAfterDefault();
                            break;
                        default:
                            ProcessAction(1);
                            break;
                    }
                    break;
            }
        }
    }

    private void ProcessNewlineAfterDefault()
    {
        switch (_currentChar)
        {
            case '}': case ']': case ')':
            case '+': case '-': case '"': case '\'':
                ProcessAction(1);
                break;
            default:
                ProcessAction(IsAlphanumeric(_currentChar) ? 1 : 3);
                break;
        }
    }

    private int GetNextCharacter()
    {
        int ch = _lookahead;
        _lookahead = Eof;

        return ch == Eof ? _reader.Read() : ch;
    }

    private int PeekNextCharacter()
    {
        return _lookahead = GetNextCharacter();
    }

    private int ReadNextCharacter()
    {
        int ch = GetNextCharacter();
        return ch == '/' ? HandleSlash() : ch;
    }

    private int HandleSlash()
    {
        switch (PeekNextCharacter())
        {
            case '/':
                return SkipSingleLineComment();
            case '*':
                return SkipBlockComment();
            default:
                return '/';
        }
    }

    private int SkipSingleLineComment()
    {
        while (GetNextCharacter() > '\n') { }
        return '\n';
    }

    private int SkipBlockComment()
    {
        GetNextCharacter();
        while (true)
        {
            switch (GetNextCharacter())
            {
                case '*':
                    if (PeekNextCharacter() == '/')
                    {
                        GetNextCharacter();
                        return ' ';
                    }
                    break;
                case Eof:
                    throw new CompressionException("Unterminated comment.");
            }
        }
    }

    private void ProcessAction(int action)
    {
        switch (action)
        {
            case 1:
                WriteCharacter(_currentChar);
                goto case 2;
            case 2:
                _currentChar = _nextChar;
                if (_currentChar is '\'' or '"')
                {
                    ProcessStringLiteral();
                }
                goto case 3;
            case 3:
                _nextChar = ReadNextCharacter();
                if (_nextChar == '/' && (_currentChar == '(' || _currentChar == ',' || _currentChar == '='))
                {
                    ProcessRegexLiteral();
                }
                break;
        }
    }

    private void ProcessStringLiteral()
    {
        while (true)
        {
            WriteCharacter(_currentChar);
            _currentChar = GetNextCharacter();

            if (_currentChar == _nextChar) break;

            if (_currentChar <= '\n')
            {
                throw new CompressionException($"Unterminated string literal: '{_currentChar}'.");
            }

            if (_currentChar == '\\')
            {
                WriteCharacter(_currentChar);
                _currentChar = GetNextCharacter();
            }
        }
    }

    private void ProcessRegexLiteral()
    {
        WriteCharacter(_currentChar);
        WriteCharacter(_nextChar);

        while ((_currentChar = GetNextCharacter()) != '/')
        {
            switch (_currentChar)
            {
                case '\\':
                    WriteCharacter(_currentChar);
                    _currentChar = GetNextCharacter();
                    break;
                case <= '\n':
                    throw new CompressionException("Unterminated Regular Expression literal.");
            }

            WriteCharacter(_currentChar);
        }

        _nextChar = ReadNextCharacter();
    }

    private void WriteCharacter(int ch) => _writer.Write((char)ch);

    private static bool IsAlphanumeric(int ch)
    {
        return ch is >= 'a' and <= 'z' or >= '0' and <= '9' or >= 'A' and <= 'Z' or '_' or '$' or '\\' or > 126;
    }

    public class CompressionException(string message) : Exception(message);
}
