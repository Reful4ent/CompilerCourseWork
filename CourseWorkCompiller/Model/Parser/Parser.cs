using System.Windows.Documents;

namespace CourseWorkCompiller.Model.Parser;

public class Parser
{
    List<ErrorToken> errors = new List<ErrorToken>();
    List<List<Token>> tokens = new List<List<Token>>();
    
    public List<ErrorToken> StartParse(List<Token> tokensGet)
    {
        errors.Clear();
        tokens.Clear();
        getLinesTokens(tokensGet);
        foreach (var listTokens in tokens)
        {
            OneListTokenRecursiveParser parser = new OneListTokenRecursiveParser(listTokens);
            errors.AddRange(parser.parse());
        }
        return errors;
    }

    public void getLinesTokens(List<Token> tokensList)
    {
        tokens = tokensList.GroupBy(ter => ter.Line)
            .Select(group => group.ToList())
            .ToList();
    }
}

partial class OneListTokenRecursiveParser
{
    private List<Token> tokensList = new List<Token>();

    public OneListTokenRecursiveParser(List<Token> _tokensList)
    {
        tokensList = _tokensList;
    }

    public List<ErrorToken> parse()
    {
        int currentPosition = 0;
        List<ErrorToken> errors = new List<ErrorToken>();
        return start(currentPosition, errors);
    }


    private List<ErrorToken> start(int currentPosition, List<ErrorToken> errors)
    {
        if (currentPosition >= tokensList.Count)
        {
            return errors;
        }
        
        currentPosition = skipNotValid(currentPosition,errors);
        if (tokensList[currentPosition].Code == TokenTypeEnum.Space)
        {
            return start(currentPosition + 1, errors);
        }

        TokenTypeEnum currentType = tokensList[currentPosition].Code;
        if (currentType == TokenTypeEnum.Int ||
            currentType == TokenTypeEnum.Char ||
            currentType == TokenTypeEnum.Float ||
            currentType == TokenTypeEnum.Void)
        {
            return spaceAfterTypeFunc(currentPosition + 1, errors);
        }
        
        return GetMinErrorList(
            spaceAfterTypeFunc(currentPosition, CreateErrorListType(currentPosition, ErrorTokenType.PUSH, errors, tokensList[currentPosition].Line)),
            spaceAfterTypeFunc(currentPosition + 1, CreateErrorListType(currentPosition, ErrorTokenType.REPLACE, errors, tokensList[currentPosition].Line)),
            start(currentPosition + 1, CreateErrorListType(currentPosition, ErrorTokenType.DELETE, errors, tokensList[currentPosition].Line))
            );
    }


    private List<ErrorToken> spaceAfterTypeFunc(int currentPosition, List<ErrorToken> errors)
    {
        if (currentPosition >= tokensList.Count)
        {
            return errors;
        }
        
        currentPosition = skipNotValid(currentPosition,errors);
        if (tokensList[currentPosition].Code != TokenTypeEnum.Space)
        {
            return GetMinErrorList(
                funcName(currentPosition,
                    CreateErrorList(currentPosition, TokenTypeEnum.Space, ErrorTokenType.PUSH, errors,
                        tokensList[currentPosition].Line)),
                funcName(currentPosition + 1,
                    CreateErrorList(currentPosition, TokenTypeEnum.Space, ErrorTokenType.REPLACE, errors,
                        tokensList[currentPosition].Line)),
                spaceAfterTypeFunc(currentPosition + 1,
                    CreateErrorList(currentPosition, TokenTypeEnum.Space, ErrorTokenType.DELETE, errors,
                        tokensList[currentPosition].Line))
            );
        }
        
        return funcName(currentPosition + 1, errors);
    }

    private List<ErrorToken> funcName(int currentPosition, List<ErrorToken> errors)
    {
        if (currentPosition >= tokensList.Count)
        {
            return errors;
        }
        currentPosition = skipNotValid(currentPosition, errors);
        if (tokensList[currentPosition].Code == TokenTypeEnum.Space)
        {
            return funcName(currentPosition + 1, errors);
        }

        if (tokensList[currentPosition].Code != TokenTypeEnum.Identifier)
        {
            return GetMinErrorList(
                openParenthesis(currentPosition,
                    CreateErrorList(currentPosition, TokenTypeEnum.Identifier, ErrorTokenType.PUSH, errors,
                        tokensList[currentPosition].Line)),
                openParenthesis(currentPosition + 1,
                    CreateErrorList(currentPosition, TokenTypeEnum.Identifier, ErrorTokenType.REPLACE, errors,
                        tokensList[currentPosition].Line)),
                funcName(currentPosition + 1,
                    CreateErrorList(currentPosition, TokenTypeEnum.Identifier, ErrorTokenType.DELETE, errors,
                        tokensList[currentPosition].Line))
            );
        }
        return openParenthesis(currentPosition + 1, errors);
    }


    private List<ErrorToken> openParenthesis(int currentPosition, List<ErrorToken> errors)
    {
        if (currentPosition >= tokensList.Count)
        {
            return errors;
        }
        
        currentPosition = skipNotValid(currentPosition, errors);
        if (tokensList[currentPosition].Code == TokenTypeEnum.OpenParenthesis && tokensList[currentPosition + 1].Code == TokenTypeEnum.Semicolon)
        {
            errors.Add(new ErrorToken("Вставить лексему: ')'", tokensList[currentPosition].Line, currentPosition, ErrorTokenType.PUSH));
            return semicolon(currentPosition + 1, errors);
        }
        if (tokensList[currentPosition].Code != TokenTypeEnum.OpenParenthesis)
        {
            return GetMinErrorList(
                nextAfterOpenParenthesis(currentPosition, CreateErrorList(currentPosition, TokenTypeEnum.OpenParenthesis, ErrorTokenType.PUSH, errors, tokensList[currentPosition].Line)),
                nextAfterOpenParenthesis(currentPosition + 1, CreateErrorList(currentPosition, TokenTypeEnum.OpenParenthesis, ErrorTokenType.REPLACE, errors, tokensList[currentPosition].Line)),
                openParenthesis(currentPosition + 1, CreateErrorList(currentPosition, TokenTypeEnum.OpenParenthesis, ErrorTokenType.DELETE, errors, tokensList[currentPosition].Line))
            );
        }
        
        return nextAfterOpenParenthesis(currentPosition + 1, errors);
    }

    private List<ErrorToken> nextAfterOpenParenthesis(int currentPosition, List<ErrorToken> errors)
    {
        if (currentPosition >= tokensList.Count)
        {
            return errors;
        }
        currentPosition = skipNotValid(currentPosition, errors);
        if (tokensList[currentPosition].Code == TokenTypeEnum.CloseParenthesis)
        {
            return semicolon(currentPosition + 1, errors);
        } else if (tokensList[currentPosition].Code == TokenTypeEnum.Int ||
                   tokensList[currentPosition].Code == TokenTypeEnum.Char ||
                   tokensList[currentPosition].Code == TokenTypeEnum.Float ||
                   tokensList[currentPosition].Code == TokenTypeEnum.Void)
        {
            return spaceAfterArgType(currentPosition + 1, errors);
        } else if ((currentPosition + 1) < tokensList.Count && tokensList[currentPosition].Code == TokenTypeEnum.Identifier && tokensList[currentPosition + 1].Code != TokenTypeEnum.Semicolon)
        {
            return argType(currentPosition, errors);
        }

        return GetMinErrorList(
            semicolon(currentPosition, CreateErrorList(currentPosition, TokenTypeEnum.CloseParenthesis, ErrorTokenType.PUSH, errors, tokensList[currentPosition].Line)),
            semicolon(currentPosition + 1, CreateErrorList(currentPosition, TokenTypeEnum.CloseParenthesis, ErrorTokenType.REPLACE, errors, tokensList[currentPosition].Line)),
            nextAfterOpenParenthesis(currentPosition + 1, CreateErrorList(currentPosition, TokenTypeEnum.CloseParenthesis, ErrorTokenType.DELETE, errors, tokensList[currentPosition].Line))
        );
    }

    private List<ErrorToken> semicolon(int currentPosition, List<ErrorToken> errors)
    {
        currentPosition = skipNotValid(currentPosition, errors);
        if (currentPosition >= tokensList.Count)
        {
            errors.Add(
                new ErrorToken(
                    CreateErrorMessage(TokenTypeEnum.Semicolon, ErrorTokenType.PUSH, currentPosition),
                    tokensList[currentPosition-1].Line,
                    currentPosition-1, ErrorTokenType.PUSH
            ));
            return errors;
        }

        if (tokensList[currentPosition].Code == TokenTypeEnum.Space)
        {
            return semicolon(currentPosition + 1, errors);
        }

        if (tokensList[currentPosition].Code != TokenTypeEnum.Semicolon)
        {
            errors.Add(
                new ErrorToken(
                    CreateErrorMessage(TokenTypeEnum.Semicolon, ErrorTokenType.DELETE, currentPosition),
                    tokensList[currentPosition].Line,
                    currentPosition,
                    ErrorTokenType.DELETE));
            semicolon(currentPosition + 1, errors);
        }
        return errors;
    }

    private List<ErrorToken> spaceAfterArgType(int currentPosition, List<ErrorToken> errors)
    {
        if (currentPosition >= tokensList.Count)
        {
            return errors;
        }
        currentPosition = skipNotValid(currentPosition, errors);
        if (tokensList[currentPosition].Code != TokenTypeEnum.Space)
        {
            return GetMinErrorList(
                argName(currentPosition, CreateErrorList(currentPosition, TokenTypeEnum.Space, ErrorTokenType.PUSH, errors, tokensList[currentPosition].Line)),
                argName(currentPosition + 1, CreateErrorList(currentPosition, TokenTypeEnum.Space, ErrorTokenType.REPLACE, errors, tokensList[currentPosition].Line)),
                spaceAfterArgType(currentPosition + 1, CreateErrorList(currentPosition, TokenTypeEnum.Space, ErrorTokenType.DELETE, errors, tokensList[currentPosition].Line))
            );
        }
        return argName(currentPosition + 1, errors);
    }

    private List<ErrorToken> argName(int currentPosition, List<ErrorToken> errors)
    {
        if (currentPosition >= tokensList.Count)
        {
            return errors;
        }
        currentPosition = skipNotValid(currentPosition, errors);
        if (tokensList[currentPosition].Code == TokenTypeEnum.Space)
        {
            return argName(currentPosition + 1, errors);
        }

        if (tokensList[currentPosition].Code != TokenTypeEnum.Identifier)
        {
            return GetMinErrorList(
                nextAfterArgName(currentPosition, CreateErrorList(currentPosition, TokenTypeEnum.Identifier, ErrorTokenType.PUSH, errors, tokensList[currentPosition].Line)),
                nextAfterArgName(currentPosition + 1, CreateErrorList(currentPosition, TokenTypeEnum.Identifier, ErrorTokenType.REPLACE, errors, tokensList[currentPosition].Line)),
                argName(currentPosition + 1, CreateErrorList(currentPosition, TokenTypeEnum.Identifier, ErrorTokenType.DELETE, errors, tokensList[currentPosition].Line))
                );
        }

        return nextAfterArgName(currentPosition + 1, errors);
    }

    private List<ErrorToken> nextAfterArgName(int currentPosition, List<ErrorToken> errors)
    {
        if (currentPosition >= tokensList.Count)
        {
            return errors;
        }
        
        if (tokensList[currentPosition].Code == TokenTypeEnum.Comma)
        {
            return spaceComma(currentPosition + 1, errors);
        }
        else if(tokensList[currentPosition].Code == TokenTypeEnum.Int ||
                tokensList[currentPosition].Code == TokenTypeEnum.Char ||
                tokensList[currentPosition].Code == TokenTypeEnum.Float ||
                tokensList[currentPosition].Code == TokenTypeEnum.Void)
        {
            return spaceAfterArgType(currentPosition + 1, errors);
        }
        else if (tokensList[currentPosition].Code == TokenTypeEnum.CloseParenthesis)
        {
            return semicolon(currentPosition + 1, errors);
        }
        if (tokensList[currentPosition].Code == TokenTypeEnum.Space || tokensList[currentPosition].Code == TokenTypeEnum.Invalid)
        {
            return comma(currentPosition, errors);
        }

        if (tokensList[currentPosition].Code == TokenTypeEnum.Semicolon)
        {
            errors.Add(new ErrorToken("Вставить лексему: ')'", tokensList[currentPosition].Line, currentPosition, ErrorTokenType.PUSH));
            return semicolon(currentPosition, errors);
        }

        return GetMinErrorList(
            semicolon(currentPosition, CreateErrorList(currentPosition, TokenTypeEnum.CloseParenthesis, ErrorTokenType.PUSH, errors, tokensList[currentPosition].Line)),
            semicolon(currentPosition + 1, CreateErrorList(currentPosition, TokenTypeEnum.CloseParenthesis, ErrorTokenType.REPLACE, errors, tokensList[currentPosition].Line)),
            nextAfterArgName(currentPosition + 1, CreateErrorList(currentPosition, TokenTypeEnum.CloseParenthesis, ErrorTokenType.DELETE, errors, tokensList[currentPosition].Line))
            );
    }
    

    private List<ErrorToken> comma(int currentPosition, List<ErrorToken> errors)
    {
        if (currentPosition >= tokensList.Count)
        {
            return errors;
        }
        
        if (currentPosition + 1 < tokensList.Count && tokensList[currentPosition + 1].Code == TokenTypeEnum.CloseParenthesis && tokensList[currentPosition].Code == TokenTypeEnum.Space)
        {
            return nextAfterArgName(currentPosition + 1, errors);
        }
        
        if (tokensList[currentPosition].Code == TokenTypeEnum.Comma)
        {
            return spaceComma(currentPosition + 1, errors);
        }
        
        return GetMinErrorList(
            spaceComma(currentPosition, CreateErrorList(currentPosition, TokenTypeEnum.Comma, ErrorTokenType.PUSH, errors, tokensList[currentPosition].Line)),
            spaceComma(currentPosition + 1, CreateErrorList(currentPosition, TokenTypeEnum.Comma, ErrorTokenType.REPLACE, errors, tokensList[currentPosition].Line)),
            comma(currentPosition + 1, CreateErrorList(currentPosition, TokenTypeEnum.Comma, ErrorTokenType.DELETE, errors, tokensList[currentPosition].Line))
        );
    }

    private List<ErrorToken> spaceComma(int currentPosition, List<ErrorToken> errors)
    {
        if (currentPosition >= tokensList.Count)
        {
            return errors;
        }
        
        if (tokensList[currentPosition].Code == TokenTypeEnum.Space)
        {
            return argType(currentPosition + 1, errors);
        }
        
        return GetMinErrorList(
            argType(currentPosition, CreateErrorList(currentPosition, TokenTypeEnum.Space, ErrorTokenType.PUSH, errors, tokensList[currentPosition].Line)),
            argType(currentPosition + 1, CreateErrorList(currentPosition, TokenTypeEnum.Space, ErrorTokenType.REPLACE, errors, tokensList[currentPosition].Line)),
            spaceComma(currentPosition + 1, CreateErrorList(currentPosition, TokenTypeEnum.Space, ErrorTokenType.DELETE, errors, tokensList[currentPosition].Line))
        );
    }

    private List<ErrorToken> argType(int currentPosition, List<ErrorToken> errors)
    {
        if (currentPosition >= tokensList.Count)
        {
            return errors;
        }
        
        currentPosition = skipNotValid(currentPosition, errors);
        if (currentPosition >= tokensList.Count)
        {
            return errors;
        }
        if (tokensList[currentPosition].Code == TokenTypeEnum.Int ||
            tokensList[currentPosition].Code == TokenTypeEnum.Char ||
            tokensList[currentPosition].Code == TokenTypeEnum.Float ||
            tokensList[currentPosition].Code == TokenTypeEnum.Void)
        {
            return spaceAfterArgType(currentPosition + 1, errors);
        }

        return GetMinErrorList(
            spaceAfterArgType(currentPosition, CreateErrorListType(currentPosition, ErrorTokenType.PUSH, errors, tokensList[currentPosition].Line)),
            spaceAfterArgType(currentPosition + 1, CreateErrorListType(currentPosition, ErrorTokenType.REPLACE, errors, tokensList[currentPosition].Line)),
            argType(currentPosition + 1, CreateErrorListType(currentPosition, ErrorTokenType.DELETE, errors, tokensList[currentPosition].Line))
        );
    }

    private int skipNotValid(int currentPosition, List<ErrorToken> errors)
    {
        while (currentPosition < tokensList.Count && tokensList[currentPosition].Code == TokenTypeEnum.Invalid)
        {
            errors.Add(new ErrorToken($"Удалить недопустимый символ: ${tokensList[currentPosition].Term}", tokensList[currentPosition].Line, tokensList[currentPosition].StartIndex, ErrorTokenType.DELETE));
            currentPosition++;
        }
        return currentPosition;
    }

    private List<ErrorToken> GetMinErrorList(List<ErrorToken> errorsFirst, List<ErrorToken> errorsSecond,
        List<ErrorToken> errorsThird)
    {
        if (errorsFirst.Count < errorsSecond.Count && errorsFirst.Count < errorsThird.Count)
        {
            return errorsFirst;
        }
        else if (errorsSecond.Count < errorsThird.Count && errorsSecond.Count < errorsFirst.Count)
        {
            return errorsSecond;
        }
        return errorsThird;
    }

    private List<ErrorToken> CreateErrorList(int currentPosition, TokenTypeEnum type, ErrorTokenType errorType, List<ErrorToken> errorsCurentState, int currentLine)
    {
        List<ErrorToken> errorsNew = new List<ErrorToken>(errorsCurentState);
        errorsNew.Add(new ErrorToken(CreateErrorMessage(type, errorType, currentPosition), currentLine, currentPosition, errorType));
        return errorsNew;
    }
    
    private List<ErrorToken> CreateErrorListType(int currentPosition, ErrorTokenType errorType, List<ErrorToken> errorsCurentState, int currentLine)
    {
        List<ErrorToken> errorsNew = new List<ErrorToken>(errorsCurentState);
        errorsNew.Add(new ErrorToken(CreateErrorMessageType(errorType, currentPosition), currentLine, currentPosition, errorType));
        return errorsNew;
    }
    
    private string CreateErrorMessageType(ErrorTokenType errorType, int currentPosition)
    {
        string errorMessage = string.Empty;
        switch (errorType)
        {
            case ErrorTokenType.PUSH:
                errorMessage = $"Вставить лексему:  'char' или 'int' или 'float' или 'void'";
                break;
            case ErrorTokenType.REPLACE:
                errorMessage = $"Заменить лексему: {tokensList[currentPosition].Term} на лексему: 'char' или 'int' или 'float' или 'void'";
                break;
            case ErrorTokenType.DELETE:
                errorMessage = $"Удалить недопустимый символ '{tokensList[currentPosition].Term}'";
                break;
        }
        return errorMessage;
    }
    
    private string CreateErrorMessage(TokenTypeEnum type, ErrorTokenType errorType, int currentPosition)
    {
        string errorMessage = string.Empty;
        string lexeme;
        if (type == TokenTypeEnum.Identifier)
        {
            lexeme = type.ToString();
        }
        else
        {
            lexeme = Lexemes.getLexemes(type);
        }
        
        switch (errorType)
        {
           case ErrorTokenType.PUSH:
               errorMessage = $"Вставить лексему: '{lexeme}'";
               break;
           case ErrorTokenType.REPLACE:
               errorMessage = $"Заменить лексему: '{tokensList[currentPosition].Term}' на лексему: '{lexeme}'";
               break;
           case ErrorTokenType.DELETE:
               errorMessage = $"Удалить недопустимый символ '{tokensList[currentPosition].Term}'";
               break;
        }
        return errorMessage;
    }
}