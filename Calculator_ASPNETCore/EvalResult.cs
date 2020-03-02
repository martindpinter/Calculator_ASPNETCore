using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calculator_ASPNETCore
{
    public class EvalResult
    {
        public string CalcResult;

        public ErrorCodes ErrorCode;
        public string ErrorMessage;
    }

    //public class ErrorObj
    //{
    //    public ErrorCodes Code;
    //    public string Message;
    //}

    public enum ErrorCodes
    {
        OK,
        InvalidSyntax
    }
}
