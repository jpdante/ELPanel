using System;
using System.IO;
using ELPanel.Manager;
using HtcSharp.Core;
using HtcSharp.Core.Helpers.Http;
using HtcSharp.Core.Interfaces.Plugin;
using HtcSharp.Core.Models.Http;
using HtcSharp.Core.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ELPanel {
    public class HtcPlugin : IHtcPlugin, IHttpEvents {

        public string PluginName => "ELPanel";        public string PluginVersion => "0.1.0";
        public string MySqlString = "";
        public static MySqlManager MySqlManager;
        public static ServerManager ServerManager;

        public void OnLoad() {
            var path = Path.Combine(HtcServer.Context.PluginsPath, "ELPanel.conf");            if (!File.Exists(path)) {
				using var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
				using var streamWriter = new StreamWriter(fileStream);
				streamWriter.Write(JsonConvert.SerializeObject(new {
                    MySqlString = "Server=127.0.0.1;Port=3306;Database=elpanel;Uid=root;Pwd=root;"
                }, Formatting.Indented));
			}            var config = IoUtils.GetJsonFile(path);            MySqlString = config.GetValue("MySqlString", StringComparison.CurrentCultureIgnoreCase).Value<string>();
            MySqlManager = new MySqlManager(MySqlString);
            ServerManager = new ServerManager();        }        public void OnEnable() {            UrlMapper.RegisterPluginPage("api/start", this);
            UrlMapper.RegisterPluginPage("api/stop", this);
            UrlMapper.RegisterPluginPage("api/restart", this);
            UrlMapper.RegisterPluginPage("api/kill", this);
            UrlMapper.RegisterPluginPage("api/getlog", this);
            UrlMapper.RegisterPluginPage("api/getstats", this);
            ServerManager.LoadServers();
        }        public void OnDisable() {
            UrlMapper.UnRegisterPluginPage("api/start");
            UrlMapper.UnRegisterPluginPage("api/stop");
            UrlMapper.UnRegisterPluginPage("api/restart");
            UrlMapper.UnRegisterPluginPage("api/kill");
            UrlMapper.UnRegisterPluginPage("api/getlog");
            UrlMapper.UnRegisterPluginPage("api/getstats");        }        public bool OnHttpPageRequest(HtcHttpContext httpContext, string filename) {                        return false;        }        public bool OnHttpExtensionRequest(HtcHttpContext httpContext, string filename, string extension) {            return false;        }

    }
}
