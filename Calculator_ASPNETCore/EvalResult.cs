using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calculator_ASPNETCore
{
    public class EvalResult
    {
        public string CalcResult;
        public string InputInPostfix;

        public ErrorCodes ErrorCode;
    }

    public enum ErrorCodes
    {
        OK,
        InvalidCharacter,
        IncompleteExpression,
        ParsingError
    }
}
