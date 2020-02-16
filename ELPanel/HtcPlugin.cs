using System;
using System.IO;
using System.Reflection;
using ELPanel.Manager;
using ELPanel.Model;
using ELPanel.Page;
using ELPanel.Util;
using HtcSharp.Core;
using HtcSharp.Core.Helpers.Http;
using HtcSharp.Core.Interfaces.Plugin;
using HtcSharp.Core.Logging;
using HtcSharp.Core.Models.Http;
using HtcSharp.Core.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ELPanel {
    public class HtcPlugin : IHtcPlugin, IHttpEvents {

        public static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        public string PluginName => "ELPanel";        public string PluginVersion => "0.1.0";
        public string MySqlString = "";
        public static MySqlManager MySqlManager;
        public static ServerManager ServerManager;
        public static SessionManager SessionManager;

        public void OnLoad() {
            var path = Path.Combine(HtcServer.Context.PluginsPath, "ELPanel.conf");            if (!File.Exists(path)) {
				using var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
				using var streamWriter = new StreamWriter(fileStream);
				streamWriter.Write(JsonConvert.SerializeObject(new {
                    MySqlString = "Server=127.0.0.1;Port=3306;Database=elpanel;Uid=root;Pwd=root;"
                }, Formatting.Indented));
			}            var config = IoUtils.GetJsonFile(path);            MySqlString = config.GetValue("MySqlString", StringComparison.CurrentCultureIgnoreCase).Value<string>();
            MySqlManager = new MySqlManager(MySqlString);
            ServerManager = new ServerManager();
            SessionManager = new SessionManager();        }        public void OnEnable() {            UrlMapper.RegisterPluginPage("/api/start", this);
            UrlMapper.RegisterPluginPage("/api/stop", this);
            UrlMapper.RegisterPluginPage("/api/restart", this);
            UrlMapper.RegisterPluginPage("/api/kill", this);
            UrlMapper.RegisterPluginPage("/api/getlog", this);
            UrlMapper.RegisterPluginPage("/api/getstats", this);
            UrlMapper.RegisterPluginPage("/api/login", this);
            ServerManager.LoadServers();
            SessionManager.Start();
        }        public void OnDisable() {
            UrlMapper.UnRegisterPluginPage("/api/start");
            UrlMapper.UnRegisterPluginPage("/api/stop");
            UrlMapper.UnRegisterPluginPage("/api/restart");
            UrlMapper.UnRegisterPluginPage("/api/kill");
            UrlMapper.UnRegisterPluginPage("/api/getlog");
            UrlMapper.UnRegisterPluginPage("/api/getstats");
            UrlMapper.UnRegisterPluginPage("/api/login");
            SessionManager.Stop();        }        public bool OnHttpPageRequest(HtcHttpContext httpContext, string filename) {
            Session session = null;			if(httpContext.Request.Cookies.TryGetValue("elpanel-session", out string value)) {
                if (SessionManager.HasSession(value)) {
                    session = SessionManager.GetSession(value);
                } else {
                    httpContext.Request.Cookies.Remove("elpanel-session");
                    httpContext.Response.Cookies.Delete("elpanel-session");
                }
            }            switch(filename.ToLower()) {
                case "/api/start":
					StartServer.OnRequest(httpContext, session);
                    break;
                case "/api/stop":
                    StopServer.OnRequest(httpContext, session);
                    break;
                case "/api/restart":
                    RestartServer.OnRequest(httpContext, session);
                    break;
                case "/api/kill":
                    KillServer.OnRequest(httpContext, session);
                    break;
                case "/api/getlog":
                    GetServerLog.OnRequest(httpContext, session);
                    break;
                case "/api/getstats":
                    GetServerStatus.OnRequest(httpContext, session);
                    break;
                case "/api/login":
                    Login.OnRequest(httpContext, session);
                    break;
            }
            return false;        }        public bool OnHttpExtensionRequest(HtcHttpContext httpContext, string filename, string extension) {            return false;        }

    }
}
