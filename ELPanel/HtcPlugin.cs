using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ELPanel.Manager;
using ELPanel.Model;
using ELPanel.Page;
using ELPanel.Util;
using HtcSharp.Core;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.Core.Plugin;
using HtcSharp.Core.Plugin.Abstractions;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ELPanel {
    public class HtcPlugin : IPlugin, IHttpEvents {

        public string Name => "ELPanel";        public string Version => "0.1.0";
        public string MySqlString = "";
        public static MySqlManager MySqlManager;
        public static ServerManager ServerManager;
        public static SessionManager SessionManager;
        public static ILogger Logger;

        public Task Load(PluginServerContext pluginServerContext, ILogger logger) {
            Logger = logger;
            string path = Path.Combine(pluginServerContext.PluginsPath, "ELPanel.conf");
            if (!File.Exists(path)) {
                using var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                using var streamWriter = new StreamWriter(fileStream);
                streamWriter.Write(JsonConvert.SerializeObject(new {
                    MySqlString = "Server=127.0.0.1;Port=3306;Database=elpanel;Uid=root;Pwd=root;"
                }, Formatting.Indented));
            }
            var config = JsonUtils.GetJsonFile(path);
            MySqlString = config.GetValue("MySqlString", StringComparison.CurrentCultureIgnoreCase).Value<string>();
            MySqlManager = new MySqlManager(MySqlString);
            ServerManager = new ServerManager();
            SessionManager = new SessionManager();
            SessionManager.AddSession("86b13566-91ff-4ae3-b92e-aa1887b87574");
            return Task.CompletedTask;
        }

        public async Task Enable() {            UrlMapper.RegisterPluginPage("/api/start", this);
            UrlMapper.RegisterPluginPage("/api/stop", this);
            UrlMapper.RegisterPluginPage("/api/restart", this);
            UrlMapper.RegisterPluginPage("/api/kill", this);
            UrlMapper.RegisterPluginPage("/api/getlog", this);
            UrlMapper.RegisterPluginPage("/api/getstats", this);
            UrlMapper.RegisterPluginPage("/api/getserver", this);
            UrlMapper.RegisterPluginPage("/api/getservers", this);
            UrlMapper.RegisterPluginPage("/api/login", this);
            await ServerManager.LoadServers();
            SessionManager.Start();
        }        public Task Disable() {
            UrlMapper.UnRegisterPluginPage("/api/start");
            UrlMapper.UnRegisterPluginPage("/api/stop");
            UrlMapper.UnRegisterPluginPage("/api/restart");
            UrlMapper.UnRegisterPluginPage("/api/kill");
            UrlMapper.UnRegisterPluginPage("/api/getlog");
            UrlMapper.UnRegisterPluginPage("/api/getstats");
            UrlMapper.UnRegisterPluginPage("/api/getserver");
            UrlMapper.UnRegisterPluginPage("/api/getservers");
            UrlMapper.UnRegisterPluginPage("/api/login");
            SessionManager.Stop();
            return Task.CompletedTask;
        }

        public bool IsCompatible(int htcMajor, int htcMinor, int htcPatch) {
            return true;
        }

        public async Task OnHttpPageRequest(HttpContext httpContext, string filename) {
            Session session = null;
            if (httpContext.Request.Headers.TryGetValue("Authorization", out var value)) {
                string[] data = value[0].Split(" ", 2);
                if (data.Length == 2 && data[0].Equals("Bearer")) {
                    session = SessionManager.GetSession(data[1]);
                }
            }
            switch (filename.ToLower()) {
                case "/api/start":
                    await StartServer.OnRequest(httpContext, session);
                    break;
                case "/api/stop":
                    await StopServer.OnRequest(httpContext, session);
                    break;
                case "/api/restart":
                    await RestartServer.OnRequest(httpContext, session);
                    break;
                case "/api/kill":
                    await KillServer.OnRequest(httpContext, session);
                    break;
                case "/api/getlog":
                    await GetServerLog.OnRequest(httpContext, session);
                    break;
                case "/api/getstats":
                    await GetServerStatus.OnRequest(httpContext, session);
                    break;
                case "/api/getservers":
                    await GetServers.OnRequest(httpContext, session);
                    break;
                case "/api/login":
                    await Login.OnRequest(httpContext);
                    break;
            }
        }

        public Task OnHttpExtensionRequest(HttpContext httpContext, string filename, string extension) {
            return Task.CompletedTask;
        }
    }
}
