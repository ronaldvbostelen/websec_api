using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Client_tech_resversi_api.Assets.Extensions
{
    public static class Extensions
    {
        public static void LogMsg<T>(this ILogger<T> logger, HttpContext context, string msg)
        {
            logger.LogInformation($"[{context.Connection.RemoteIpAddress}] {context.Request.Method} " +
                                   $"{context.Request.Path} {context.GetRouteValue("controller")}: {context.GetRouteValue("action")} {context.User.Claims.FirstOrDefault(x => x.Type == "UserId")} -- {msg}");
        }
    }
}
