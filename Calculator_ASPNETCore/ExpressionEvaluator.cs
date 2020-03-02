using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calculator_ASPNETCore
{

    public enum TokenType
    {
        Number,
        Add,
        Substract,
        Multiply,
        Divide,
        Power,
        OpenParen,
        CloseParen,
    }

    public enum StateType
    {
        NewToken,
        ParsingNumber,
        ParsingOperator,
    }

    public class Token
    {
        public TokenType Type;
        public string Value;
    }

    public class TokenOpResult
    {
        public List<Token> Tokens;
        public ErrorCodes Error = ErrorCodes.OK;
    }

    public static class ExpressionEvaluator
    {
        public static EvalResult Evaluate(string data)
        {
            TokenOpResult tokenizeRes = Tokenize(data);
            if (tokenizeRes.Error > ErrorCodes.OK)
                return new EvalResult() { ErrorCode = tokenizeRes.Error };

            TokenOpResult postfixTokens = InfixToPostfix(tokenizeRes.Tokens);
            if (postfixTokens.Error > ErrorCodes.OK)
                return new EvalResult() { ErrorCode = postfixTokens.Error };

            string strPostfix = String.Empty;
            foreach (var t in postfixTokens.Tokens)
                strPostfix += t.Value;

            EvalResult result = CalcFromPostfix(postfixTokens.Tokens);

            return result;
            //return new EvalResult() { CalcResult = result };
        }

        private static EvalResult CalcFromPostfix(List<Token> postfix)
        {
            Stack<double> resStack = new Stack<double>();
            foreach(var t in postfix)
            {
                if (t.Type == TokenType.Number)
                {
                    Double.TryParse(t.Value, out double v);
                    resStack.Push(v);
                }
                else
                {
                    if (resStack.Count < 2)
                        return new EvalResult() { ErrorCode = ErrorCodes.InvalidSyntax };


                    double rhs = resStack.Pop();
                    double lhs = resStack.Pop();

                    switch(t.Type)
                    {
                        case TokenType.Add:
                            resStack.Push(lhs + rhs);
                            break;
                        case TokenType.Substract:
                            resStack.Push(lhs - rhs);
                            break;
                        case TokenType.Multiply:
                            resStack.Push(lhs * rhs);
                            break;
                        case TokenType.Divide:
                            resStack.Push(lhs / rhs);
                            break;
                        case TokenType.Power:
                            resStack.Push(Math.Pow(lhs, rhs));
                            break;
                    }
                }
            }

            if (resStack.Count != 1)
            {
                return new EvalResult() { ErrorCode = ErrorCodes.InvalidSyntax };
                // TODO - fix (tested with "()")
                throw new Exception($"ERROR: final stack count must be 1 ({resStack.Count})");
            }

            return new EvalResult() { CalcResult = resStack.Pop().ToString() };
        }

        /// <summary>
        /// Parses list of tokens into postfix notation using shunting yard algorithm
        /// </summary>
        /// <param name="tokenList"></param>
        /// <returns></returns>
        private static TokenOpResult InfixToPostfix(List<Token> tokenList)
        {
            List<Token> res = new List<Token>();

            Dictionary<TokenType, int> precedence = new Dictionary<TokenType, int>()
            {
                { TokenType.Add, 1 },
                { TokenType.Substract, 1 },
                { TokenType.Multiply, 2 },
                { TokenType.Divide, 2 },
                { TokenType.Power, 3 }
            };
            Stack<Token> stack = new Stack<Token>();

            foreach (var t in tokenList)
            {
                if (t.Type == TokenType.Number)
                {
                    res.Add(t);
                }
                //else if (Token is function)
                else if (t.Type == TokenType.OpenParen)
                {
                    stack.Push(t);
                }
                else if (t.Type == TokenType.CloseParen)
                {
                    while((stack.Count > 0) && (stack.Peek().Type != TokenType.OpenParen))
                    { 
                        res.Add(stack.Pop());
                    }

                    if (stack.Count > 0 && stack.Peek().Type == TokenType.OpenParen)
                    {
                        stack.Pop(); // discard
                    }
                }
                else
                {
                    while(stack.Count > 0
                        && stack.Peek().Type != TokenType.OpenParen
                        && (precedence[stack.Peek().Type] > precedence[t.Type]
                        || (precedence[stack.Peek().Type] == precedence[t.Type] && t.Type != TokenType.Power)))
                    {
                        res.Add(stack.Pop());
                    }
                    stack.Push(t);
                }
            }

            while (stack.Count > 0)
            {
                res.Add(stack.Pop());
            }

            return new TokenOpResult() { Tokens = res };
        }

        private static TokenOpResult Tokenize(string data)
        {
            List<Token> tokenList = new List<Token>();
            StateType state = StateType.NewToken;
            int tokenStart = 0;

            data += '\0';

            for (int i = 0; i < data.Length; i++)
            {
                switch (state)
                {
                    case StateType.NewToken:
                        if (Char.IsDigit(data[i]))
                        {
                            state = StateType.ParsingNumber;
                            tokenStart = i;
                        }
                        else if (data[i] == '+') { tokenList.Add(new Token() { Type = TokenType.Add, Value = "+" }); }
                        else if (data[i] == '-') { tokenList.Add(new Token() { Type = TokenType.Substract, Value = "-" }); }
                        else if (data[i] == '*') { tokenList.Add(new Token() { Type = TokenType.Multiply, Value = "*" }); }
                        else if (data[i] == '/') { tokenList.Add(new Token() { Type = TokenType.Divide, Value = "/" }); }
                        else if (data[i] == '^') { tokenList.Add(new Token() { Type = TokenType.Power, Value = "^" }); }
                        else if (data[i] == '(') { tokenList.Add(new Token() { Type = TokenType.OpenParen }); }
                        else if (data[i] == ')') { tokenList.Add(new Token() { Type = TokenType.CloseParen }); }
                        else
                        {
                            if ((data[i] != '\0') && !Char.IsWhiteSpace(data[i]))
                                return new TokenOpResult() { Error = ErrorCodes.InvalidSyntax };
                        }
                        break;

                    case StateType.ParsingNumber:
                        if (!Char.IsDigit(data[i]))
                        {
                            state = StateType.NewToken;
                            tokenList.Add(new Token()
                            {
                                Type = TokenType.Number,
                                Value = data.Substring(tokenStart, i - tokenStart) // +1?
                            });

                            i--;
                        }
                        break;
                }
            }

            return new TokenOpResult() { Tokens = tokenList };
        }
    }
}
