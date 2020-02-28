using System;
using System.Threading.Tasks;
using ELPanel.Model;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;

namespace ELPanel.Page {
	public static class StopServer {

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
#pragma warning disable 4014
                    server.Stop();
#pragma warning restore 4014
                    httpContext.Response.StatusCode = 200;
                    await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = true }));
                } else {
                    httpContext.Response.StatusCode = 200;
                    await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = false, error = 2, message = "Fields are missing." }));
                }
            } catch (Exception ex) {
                if (!httpContext.Response.HasStarted) {
                    httpContext.Response.ContentType = ContentType.TEXT.ToValue();
                    httpContext.Response.StatusCode = 503;
                    await httpContext.Response.WriteAsync("Failed to parse json request data.");
                }
                HtcPlugin.Logger.LogTrace("[ServerStatus Exception]", ex);
            }
        }
    }
}
