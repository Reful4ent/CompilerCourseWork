namespace CourseWorkCompiller.Model.Parser;

public class ErrorToken
{
    public string ErrorMessage { get; set; }
    public int Line { get; set; }
    public int Index { get; set; }
    public ErrorTokenType ErrorType { get; set; }

    public ErrorToken(string message, int line, int index, ErrorTokenType errorType)
    {
        ErrorMessage = message;
        Line = line;
        Index = index;
        ErrorType = errorType;
    }
}