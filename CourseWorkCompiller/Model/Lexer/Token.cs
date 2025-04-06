using System.Text.RegularExpressions;

namespace CourseWorkCompiller.Model;

public class Token
{
    public static Dictionary<string, (string token, TokenTypeEnum tokenType)> tokens = new Dictionary<string, (string, TokenTypeEnum)>
    {
        { @", ", ("COMMA_SPACE", TokenTypeEnum.CommaSpace) },
        { @"\bint\b", ("INT", TokenTypeEnum.Int) },
        { @"\bchar\b", ("CHAR", TokenTypeEnum.Char) },
        { @"\bvoid\b", ("VOID", TokenTypeEnum.Void) },
        { @"\bfloat\b", ("FLOAT", TokenTypeEnum.Float) },
        { @";", ("SEMICOLON", TokenTypeEnum.Semicolon) },
        { @"\(", ("OPEN_PARENTHESIS", TokenTypeEnum.OpenParenthesis) },
        { @"\)", ("CLOSE_PARENTHESIS", TokenTypeEnum.CloseParenthesis) },
        { @"[a-zA-Z][a-zA-Z0-9]*", ("IDENTIFIER", TokenTypeEnum.Identifier) },
        { @"[^\s]", ("INVALID", TokenTypeEnum.Invalid) },
        { @" ", ("SPACE", TokenTypeEnum.Space) },
    };
    public int Line {get; set;}
    public int StartIndex {get; set;}
    public int EndIndex {get; set;}
    public string Message {get; set;}
    public string Term {get; set;}
    public string UnTerm {get; set;}
    public TokenTypeEnum Code {get; set;}

    public Token(int line, int startIndex, int endIndex, string message, string text)
    {
        Line = line;
        StartIndex = startIndex;
        EndIndex = endIndex;
        Message = message;
        Term = text;
        UnTerm = ParseToken(text).Item1;
        Code = ParseToken(text).Item2;
    }


    private (string, TokenTypeEnum) ParseToken(string token)
    {
        foreach (var tokensReg in tokens)
        {
            if (Regex.IsMatch(token, tokensReg.Key))
                return tokensReg.Value;
        }
        return ("INVALID", TokenTypeEnum.Invalid);
    }
}