namespace Kanalyzer.BusinessLogic
{
    public class Lexeme
    {
        public string Value { get; set; }
        public LexemeType Type { get; set; }

        public override string ToString()
        {
            return $"{Value} {Type.ToString()}";
        }
    }
}
