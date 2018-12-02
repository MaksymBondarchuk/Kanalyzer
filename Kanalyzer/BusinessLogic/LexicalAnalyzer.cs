using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Kanalyzer.BusinessLogic
{
    public class LexicalAnalyzer
    {
        private const char NewLine = '\n';

        private List<string> PredefinedWords { get; } = new List<string>
        {
            "do", "while", "enddo", "if", "then", "fi", "read", "write", "set"
        };
        private List<char> Delimiters { get; } = new List<char> { '+', '-', '*', '/', '(', ')', '¶' };
        private List<char> Operators { get; } = new List<char> { '+', '-', '*', '/', '(', ')' };
        private List<char> TrimDelimiters { get; } = new List<char> { ' ', '\t', NewLine };

        public List<Lexeme> Lexemes { get; } = new List<Lexeme>();
        public List<string> Identifiers { get; } = new List<string>();
        public List<string> Constants { get; } = new List<string>();
        public List<LexicalError> Errors { get; } = new List<LexicalError>();

        public void Parse(string code)
        {
            var lineNumber = 0;
            var lexemeBuilder = new StringBuilder();
            foreach (char symbol in code)
            {
                if (symbol == NewLine)
                    lineNumber++;

                if (TrimDelimiters.Contains(symbol))
                    continue;

                if (Delimiters.Contains(symbol))
                {
                    var lexeme = lexemeBuilder.ToString();
                    LexemeType type = LexemeType.Unknown;
                    if (PredefinedWords.Contains(lexeme))
                        type = LexemeType.PredefinedWord;
                    else if (IsIdentifier(lexeme))
                        type = LexemeType.Identifier;
                    else if (IsConstant(lexeme))
                        type = LexemeType.Constant;
                    else if (!string.IsNullOrEmpty(lexeme))
                    {
                        Errors.Add(new LexicalError
                        {
                            Line = lineNumber,
                            Text = $"Unknown lexeme: {lexeme}"
                        });
                        lexemeBuilder.Clear();
                        continue;
                    }

                    if (!string.IsNullOrEmpty(lexeme))
                        Lexemes.Add(new Lexeme
                        {
                            Value = lexeme,
                            Type = type
                        });

                    Lexemes.Add(new Lexeme
                    {
                        Value = symbol.ToString(),
                        Type = Operators.Contains(symbol)
                            ? LexemeType.Operator
                            : LexemeType.Delimiter
                    });

                    lexemeBuilder.Clear();
                }

                lexemeBuilder.Append(symbol);
            }
        }

        private bool IsIdentifier(string lexeme)
        {
            return Regex.IsMatch(lexeme, "^@[a-zA-z]+$");
        }

        private bool IsConstant(string lexeme)
        {
            return Regex.IsMatch(lexeme, "^[0-9]([.,][0-9]{1,8})?$");
        }
    }
}
