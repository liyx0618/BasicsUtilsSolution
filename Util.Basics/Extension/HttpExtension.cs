using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Util.Basics.Extension
{
    public static class HttpExtension
    {
        public static bool IsAjax(this HttpRequest req)
        {
            bool result = false;

            var xreq = req.Headers.AllKeys.Contains("x-requested-with");
            if (xreq)
            {
                result = req.Headers["x-requested-with"] == "XMLHttpRequest";
            }

            return result;
        }
    }
}
