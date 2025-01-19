// using System.Diagnostics;
//
// namespace WF.MinifyBundler;
//
// internal sealed class OldCompressor
// {
//     private const int Eof = -1;
//     private int _previousChar;
//     private int _currentChar;
//     private int _nextChar;
//     private int _lookahead = Eof;
//     private readonly TextReader _reader;
//     private readonly TextWriter _writer;
//
//     public static string Compress(string source)
//     {
//         using var writer = new StringWriter();
//         Compress(new StringReader(source), writer);
//         return writer.ToString().Trim();
//     }
//
//     public static void Compress(TextReader reader, TextWriter writer)
//     {
//         ArgumentNullException.ThrowIfNull(reader);
//         ArgumentNullException.ThrowIfNull(writer);
//
//         new OldCompressor(reader, writer).Compress();
//     }
//
//     private OldCompressor(TextReader reader, TextWriter writer)
//     {
//         Debug.Assert(reader != null);
//         Debug.Assert(writer != null);
//
//         _reader = reader;
//         _writer = writer;
//     }
//
//     private void Compress()
//     {
//         _currentChar = '\n';
//         ProcessAction(3);
//         while (_currentChar != Eof)
//         {
//             ProcessCurrentCharacter();
//         }
//     }
//
//     private void ProcessCurrentCharacter()
//     {
//         switch (_currentChar)
//         {
//             case ' ':
//                 ProcessSpace();
//                 break;
//             case '\n':
//                 ProcessNewLine();
//                 break;
//             default:
//                 ProcessDefault();
//                 break;
//         }
//     }
//
//     private void ProcessSpace()
//     {
//         if (_previousChar is 123 or 45 or 47 or 44 or 60 or 61 or 62 or 59 or 32)
//         {
//             ProcessAction(2);
//             return;
//         }
//         
//         ProcessAction(IsAlphanumeric(_nextChar) ? 1 : 2);
//         
//     }
//     private void ProcessNewLine()
//     {
//         ProcessAction(2);
//         return;
//         switch (_nextChar)
//         {
//             case '{': case '[': case '(':
//             case '+': case '-':
//                 ProcessAction(1);
//                 break;
//             case ' ':
//                 ProcessAction(3);
//                 break;
//             default:
//                 ProcessAction(IsAlphanumeric(_nextChar) ? 1 : 3);
//                 break;
//         }
//     }
//
//     private void ProcessDefault()
//     {
//         switch (_nextChar)
//         {
//             case ' ' when IsAlphanumeric(_currentChar):
//                 ProcessAction(1);
//                 break;
//             case '\n':
//                 ProcessNewLineAfterDefault();
//                 break;
//             default:
//                 ProcessAction(1);
//                 break;
//         }
//     }
//     
//     private void ProcessNewLineAfterDefault()
//     {
//         switch (_currentChar)
//         {
//             case '}': case ']': case ')':
//             case '+': case '-': case '"': case '\'':
//                 ProcessAction(1);
//                 break;
//             default:
//                 ProcessAction(IsAlphanumeric(_currentChar) ? 1 : 3);
//                 break;
//         }
//     }
//
//     private int GetNextChar()
//     {
//         var ch = _lookahead;
//         _lookahead = Eof;
//         
//         return ch == Eof ? _reader.Read() : ch;
//     }
//
//     private int PeekNextChar()
//     {
//         return _lookahead = GetNextChar();
//     }
//     
//     private int ReadNextCharacter()
//     {
//         var ch = GetNextChar();
//         
//         return ch == '/' ? HandleSlash() : ch;
//     }
//
//     private int HandleSlash()
//     {
//         return PeekNextChar() switch
//         {
//             '/' => SkipLineComment(),
//             '*' => SkipBlockComment(),
//             _ => '/'
//         };
//     }
//
//     private int SkipLineComment()
//     {
//         while (GetNextChar() > '\n') {}
//         return '\n';
//     }
//
//     private int SkipBlockComment()
//     {
//         GetNextChar();
//         while (true)
//         {
//             switch (GetNextChar())
//             {
//                 case '*':
//                     if (PeekNextChar() == '/')
//                     {
//                         GetNextChar();
//                         
//                         return ' ';
//                     }
//                     break;
//                 case Eof:
//                     throw new CompressorException("Unterminated comment.");
//             }
//         }
//     }
//
//     private void ProcessAction(int action)
//     {
//         _previousChar = _currentChar;
//         switch (action)
//         {
//             case 1:
//                 WriteChar(_currentChar);
//                 goto case 2;
//             case 2:
//                 ProcessNextChar();
//                 goto case 3;
//             case 3:
//                 ParseRegexLiteral();
//                 break;
//         }
//     }
//
//     private void ProcessNextChar()
//     {
//         _currentChar = _nextChar;
//         if (_currentChar is '\'' or '"')
//         {
//             ProcessStringLiteral();
//         }
//     }
//
//     private void ProcessStringLiteral()
//     {
//         while (true)
//         {
//             WriteChar(_currentChar);
//             _currentChar = GetNextChar();
//             if (_currentChar == _nextChar) break;
//             
//             switch (_currentChar)
//             {
//                 case <= '\n':
//                     throw new CompressorException($"Unterminated string literal: '{_currentChar}'.");
//                 case '\\':
//                     WriteChar(_currentChar);
//                     _currentChar = GetNextChar();
//                     break;
//             }
//         }
//     }
//
//     private void ParseRegexLiteral()
//     {
//         _nextChar = ReadNextCharacter();
//         
//         if (_nextChar != '/' || !"(,=".Contains((char)_currentChar)) return;
//         
//         WriteChar(_currentChar);
//         WriteChar(_nextChar);
//         
//         while ((_currentChar = GetNextChar()) != '/')
//         {
//             switch (_currentChar)
//             {
//                 case '\\':
//                     WriteChar(_currentChar);
//                     _currentChar = GetNextChar();
//                     break;
//                 case <= '\n':
//                     throw new CompressorException("Unterminated Regular Expression literal.");
//             }
//
//             WriteChar(_currentChar);
//         }
//         
//         _nextChar = ReadNextCharacter();
//     }
//
//     private void WriteChar(int ch) => _writer.Write((char)ch);
//
//     private static bool IsAlphanumeric(int c)
//     {
//         return c is >= 'a' and <= 'z' or >= '0' and <= '9' or >= 'A' and <= 'Z' or '_' or '$' or '\\' or > 126;
//     }
//
//     public class CompressorException(string message) : Exception(message);
// }
