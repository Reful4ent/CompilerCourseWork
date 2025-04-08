namespace CourseWorkCompiller.Model;

public static class Lexemes
{
    public static Dictionary<string, TokenTypeEnum> Lexems = new Dictionary<string, TokenTypeEnum>
    {
        {",", TokenTypeEnum.Comma},
        { ";", TokenTypeEnum.Semicolon },
        { "(", TokenTypeEnum.OpenParenthesis },
        { ")", TokenTypeEnum.CloseParenthesis },
        { " ", TokenTypeEnum.Space },
    };

    public static string getLexemes(TokenTypeEnum lexeme)
    {
        foreach (var kvp in Lexems)
        {
            if (kvp.Value == lexeme)
            {
                return kvp.Key;
            }
        }
        return " ";
    }
}