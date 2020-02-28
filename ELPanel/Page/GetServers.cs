using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ELPanel.Model;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;

namespace ELPanel.Page {
	public static class GetServers {

		public static async Task OnRequest(HttpContext httpContext, Session session) {
            try {
                httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                httpContext.Response.Headers.Add("Access-Control-Allow-Methods", "*");
                httpContext.Response.Headers.Add("Access-Control-Allow-Headers", "*");
                if (session == null) {
                    httpContext.Response.StatusCode = 200;
                    httpContext.Response.ContentType = ContentType.JSON.ToValue();
                    await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = false, invalidtoken = true, message = "Invalid token." }));
                    return;
                }
                var list = new List<object>();
                foreach (var server in HtcPlugin.ServerManager.GetServers()) {
                    list.Add(new { id = server.ServerInfo.ServerID, name = server.ServerInfo.ServerName, online = server.Active, count = server.PlayersOnline, max = server.MaxPlayers });
                }
                httpContext.Response.StatusCode = 200;
                httpContext.Response.ContentType = ContentType.JSON.ToValue();
                await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = true, servers = list }));
            } catch (Exception ex) {
                if (!httpContext.Response.HasStarted) {
                    httpContext.Response.ContentType = ContentType.TEXT.ToValue();
                    httpContext.Response.StatusCode = 503;
                    await httpContext.Response.WriteAsync("Failed to parse json request data.");
                }
                HtcPlugin.Logger.LogTrace("[Login Exception]", ex);
            }
        }
    }
}
