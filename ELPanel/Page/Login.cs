using System;
using ELPanel.Model;
using ELPanel.Util;
using HtcSharp.Core.Helpers.Http;
using HtcSharp.Core.Models.Http;
using HtcSharp.Core.Utils;
using MySql.Data.MySqlClient;

namespace ELPanel.Page {
    public static class Login {

        public static void OnRequest(HtcHttpContext httpContext, Session session) {
            if (session != null) {
                httpContext.Response.StatusCode = 403;
                httpContext.Response.ContentType = ContentType.JSON.ToValue();
                httpContext.Response.Write(JsonUtils.SerializeObject(new { success = false, error = 1, message = "You cannot log in if you are already logged in." } ));
                return;
            }
            if (httpContext.Request.Post.TryGetValue("email", out string email) && httpContext.Request.Post.TryGetValue("password", out string password)) {
                var connection = HtcPlugin.MySqlManager.GetMySqlConnection();
                if (HtcPlugin.MySqlManager.CheckLogin(email, EncryptionUtils.ComputeSha512Hash(password), connection)) {
                    var id = Guid.NewGuid().ToString();
                    httpContext.Response.Cookies.Append("elpanel-session", id);
                    HtcPlugin.SessionManager.AddSession(id);
                    httpContext.Response.StatusCode = 200;
                    httpContext.Response.ContentType = ContentType.JSON.ToValue();
                    httpContext.Response.Write(JsonUtils.SerializeObject(new { success = true }));
                } else {
                    httpContext.Response.StatusCode = 403;
                    httpContext.Response.ContentType = ContentType.JSON.ToValue();
                    httpContext.Response.Write(JsonUtils.SerializeObject(new { success = false, error = 3, message = "Wrong Email or Password!" }));
                }
            } else {
                httpContext.Response.StatusCode = 403;
                httpContext.Response.ContentType = ContentType.JSON.ToValue();
                httpContext.Response.Write(JsonUtils.SerializeObject(new { success = false, error = 2, message = "Fields are missing!" }));
            }
        }

    }
}