using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Util.Basics.Extension
{
    public static class HttpContextExtension
    {
        /// <summary>
        /// 获取客户端Ip
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetClientIp(this HttpRequest request)
        {
            //var ip = request.Headers["X-Real-IP"].FirstOrDefault() ??
            //         request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var ip = request.Headers["X-Real-IP"].FirstOrDefault().ToString();
            return ip;
        }

    }
}
