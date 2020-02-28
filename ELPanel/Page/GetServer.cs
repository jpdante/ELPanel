using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ELPanel.Model;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;

namespace ELPanel.Page {
	public static class GetServer {

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
                httpContext.Response.ContentType = ContentType.JSON.ToValue();
                var data = await new JsonData().Load(httpContext);
                if (data.TryGetValue("id", out string idRaw) && int.TryParse(idRaw, out int id)) {
                    var server = HtcPlugin.ServerManager.GetServer(id);
                    if (server == null) {
                        httpContext.Response.StatusCode = 200;
                        await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = false, message = "Invalid server id." }));
                        return;
                    }
                    httpContext.Response.StatusCode = 200;
                    await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(
                        new {
                            success = true,
                            server = new {
                                id = server.ServerInfo.ServerID,
                                name = server.ServerInfo.ServerName,
                                arguments = server.ServerInfo.Arguments,
                                filename = server.ServerInfo.FileName,
                                maxmemory = server.ServerInfo.MaxMemory,
                                minmemory = server.ServerInfo.MinMemory,
                                directory = server.ServerInfo.WorkingDirectory,
                                online = server.Active,
                                count = server.PlayersOnline,
                                max = server.MaxPlayers
                            }
                        }));
                } else {
                    httpContext.Response.StatusCode = 200;
                    await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = false, error = 2, message = "Fields are missin.!" }));
                }
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
