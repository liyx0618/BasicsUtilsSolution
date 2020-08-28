using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util.Basics
{
    public class BreakOutException : Exception
    {
        public BreakOutException()
            : base()
        { }
        public BreakOutException(string message)
            : base(message)
        { }
        public BreakOutException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
