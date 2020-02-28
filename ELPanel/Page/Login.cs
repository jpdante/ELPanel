using System;
using System.Threading.Tasks;
using ELPanel.Model;
using ELPanel.Util;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;

namespace ELPanel.Page {
    public static class Login {

        public static async Task OnRequest(HttpContext httpContext) {
            try {
                httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                httpContext.Response.Headers.Add("Access-Control-Allow-Methods", "*");
                httpContext.Response.Headers.Add("Access-Control-Allow-Headers", "*");
                if (string.IsNullOrEmpty(httpContext.Request.ContentType) || !httpContext.Request.ContentType.Equals(ContentType.JSON.ToValue())) {
                    httpContext.Response.ContentType = ContentType.TEXT.ToValue();
                    await httpContext.Response.WriteAsync("Incorrect content type!");
                    return;
                }
                httpContext.Response.ContentType = ContentType.JSON.ToValue();
                var data = await new JsonData().Load(httpContext);
                if (data.TryGetValue("email", out string email) && data.TryGetValue("password", out string password)) {
                    var connection = await HtcPlugin.MySqlManager.GetMySqlConnection();
                    if (await HtcPlugin.MySqlManager.CheckLogin(email, EncryptionUtils.ComputeSha512Hash(password), connection)) {
                        var id = Guid.NewGuid().ToString();
                        //httpContext.Response.Cookies.Append("elpanel-session", id);
                        HtcPlugin.SessionManager.AddSession(id);
                        httpContext.Response.StatusCode = 200;
                        await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = true, token = id }));
                    } else {
                        httpContext.Response.StatusCode = 200;
                        await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = false, error = 3, message = "Wrong Email or Password!" }));
                    }
                } else {
                    httpContext.Response.StatusCode = 200;
                    await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = false, error = 2, message = "Fields are missing!" }));
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