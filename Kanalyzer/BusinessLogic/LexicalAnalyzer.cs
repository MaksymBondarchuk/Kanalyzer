using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Kanalyzer.BusinessLogic
{
    public class LexicalAnalyzer
    {
        private static readonly string NewLine = Environment.NewLine;

        private List<string> PredefinedWords { get; } = new List<string>
        {
            "do", "while", "enddo", "if", "then", "fi", "read", "write", "set"
        };
        private List<string> Delimiters { get; } = new List<string> { "+", "-", "*", "/", "(", ")", "¶" };
        private List<string> Operators { get; } = new List<string> { "+", "-", "*", "/", "(", ")", "<", ">", "==" };
        private List<string> TrimDelimiters { get; } = new List<string> { " ", "\r", "\t", NewLine };

        public List<Lexeme> Lexemes { get; } = new List<Lexeme>();
        public List<string> Identifiers { get; } = new List<string>();
        public List<string> Constants { get; } = new List<string>();
        public List<LexicalError> Errors { get; } = new List<LexicalError>();

        public void Parse(string code)
        {
            var lineNumber = 1;
            var lexemeBuilder = new StringBuilder();
            foreach (string symbol in code.Select(c => c.ToString()))
            {
                if (symbol.Equals("\r"))
                    lineNumber++;

                if (TrimDelimiters.Contains(symbol) || Delimiters.Contains(symbol))
                {
                    var lexeme = lexemeBuilder.ToString();
                    LexemeType type = LexemeType.Unknown;
                    if (PredefinedWords.Contains(lexeme))
                        type = LexemeType.PredefinedWord;
                    else if (IsIdentifier(lexeme))
                        type = LexemeType.Identifier;
                    else if (IsConstant(lexeme))
                        type = LexemeType.Constant;
                    else if (Operators.Contains(lexeme))
                        type = LexemeType.Operator;
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

                    if (Delimiters.Contains(symbol))
                        Lexemes.Add(new Lexeme
                        {
                            Value = symbol,
                            Type = LexemeType.Delimiter
                        });

                    lexemeBuilder.Clear();
                    continue;
                }

                if (!TrimDelimiters.Contains(symbol))
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
