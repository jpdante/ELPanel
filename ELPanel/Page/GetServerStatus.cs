using System;
using System.Threading.Tasks;
using ELPanel.Model;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using MySql.Data.MySqlClient;

namespace ELPanel.Page {
	public static class GetServerStatus {
        public static Random random = new Random(); 

		public static async Task OnRequest(HttpContext httpContext, Session session) {
            httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            httpContext.Response.Headers.Add("Access-Control-Allow-Methods", "*");
            httpContext.Response.Headers.Add("Access-Control-Allow-Headers", "*");
            httpContext.Response.StatusCode = 200;
            httpContext.Response.ContentType = ContentType.JSON.ToValue();
            await httpContext.Response.WriteAsync(JsonUtils.SerializeObject(new { success = true, cpu = random.Next(100), ram = random.Next(100) }));
        }

	}
}
