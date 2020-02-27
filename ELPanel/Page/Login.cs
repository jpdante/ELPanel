using System;
using System.Threading.Tasks;
using ELPanel.Model;
using ELPanel.Util;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using MySql.Data.MySqlClient;

namespace ELPanel.Page {
    public static class Login {

        public static async Task OnRequest(HttpContext httpContext, Session session) {
            if (session != null) {
                httpContext.Response.StatusCode = 403;
                httpContext.Response.ContentType = ContentType.JSON.ToValue();
                await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = false, error = 1, message = "You cannot log in if you are already logged in." } ));
                return;
            }
            if (httpContext.Request.Form.TryGetValue("email", out var email) && httpContext.Request.Form.TryGetValue("password", out var password)) {
                var connection = await HtcPlugin.MySqlManager.GetMySqlConnection();
                if (await HtcPlugin.MySqlManager.CheckLogin(email, EncryptionUtils.ComputeSha512Hash(password), connection)) {
                    var id = Guid.NewGuid().ToString();
                    httpContext.Response.Cookies.Append("elpanel-session", id);
                    HtcPlugin.SessionManager.AddSession(id);
                    httpContext.Response.StatusCode = 200;
                    httpContext.Response.ContentType = ContentType.JSON.ToValue();
                    await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = true }));
                } else {
                    httpContext.Response.StatusCode = 403;
                    httpContext.Response.ContentType = ContentType.JSON.ToValue();
                    await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = false, error = 3, message = "Wrong Email or Password!" }));
                }
            } else {
                httpContext.Response.StatusCode = 403;
                httpContext.Response.ContentType = ContentType.JSON.ToValue();
                await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = false, error = 2, message = "Fields are missing!" }));
            }
        }

    }
}