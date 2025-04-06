using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using CourseWorkCompiller.Model;
using CourseWorkCompiller.Model.Parser;
using CourseWorkCompiller.ViewModel.Commands;

namespace CourseWorkCompiller.ViewModel;

public class MainVM : BaseVM
{
    private string enteredCode = string.Empty;
    private string _outputText = string.Empty;
    private string indexesNumbers = "1\n";
    private int numbersCount = 1;
    private Lexer lexer = new Lexer();
    private Parser parser = new Parser();
    private ObservableCollection<ErrorToken> errors = new();
    private ObservableCollection<Token> lexemesTokens = new();
    
    public ObservableCollection<ErrorToken> Errors { get => errors; set => Set(ref errors, value); }
    public ObservableCollection<Token> LexemesTokens { get => lexemesTokens; set => Set(ref lexemesTokens, value); }
    
    public string EnteredCode
    {
        get => enteredCode;
        set
        {
            Set(ref enteredCode, value);
            //Errors.Add(new Error(1, 1 ,1, "asddas"));
            
            UpdateIndexesNumbers(value);
        }
    }
    
    public string OutputText
    {
        get => _outputText;
        set => Set(ref _outputText, value);
    }

    public string IndexesNumbers
    {
        get => indexesNumbers;
        set => Set(ref indexesNumbers, value);
    }
    
    private void UpdateIndexesNumbers(string value)
    {
        int numbersSplits = value.Split('\n').Length;
        if (numbersCount < numbersSplits)
        {
            numbersCount = numbersSplits;
            IndexesNumbers = IndexesNumbers.Split('\n')[0] + "\n";
            for (int i = 2; i < numbersSplits + 1; i++)
            {
                IndexesNumbers += $"{i}\n";
            }
        }

        if (numbersCount > numbersSplits)
        {
            numbersCount -= 1;
            IndexesNumbers = IndexesNumbers.Split('\n')[0] + "\n";
            for (int i = 2; i < numbersSplits + 1; i++)
            {
                IndexesNumbers += $"{i}\n";
            }
        }
    }
    
    public Command ClearAllCommand => Command.Create(ClearAll);
    
    public void ClearAll()
    {
        EnteredCode = string.Empty;
        OutputText = string.Empty;
    }

    public Command StarScanCommand => Command.Create(StartScan);

    public void StartScan()
    {
        string text = EnteredCode.Replace("\t", "").Replace("\r", "");
        text = Regex.Replace(text, @" {1,}", " ");
        List<Token> tokens = lexer.GetLexemes(text);
        LexemesTokens = new ObservableCollection<Token>(lexer.GetLexemes(text));
        Errors = new ObservableCollection<ErrorToken>(parser.StartParse(tokens));
    }
}