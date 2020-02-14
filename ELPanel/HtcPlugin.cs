using System;
using HtcSharp.Core.Helpers.Http;
using HtcSharp.Core.Interfaces.Plugin;
using HtcSharp.Core.Logging;
using HtcSharp.Core.Models.Http;

namespace ELPanel {
    public class HtcPlugin : IHtcPlugin, IHttpEvents {

        public string PluginName => "ElPanel";        public string PluginVersion => "0.1.0";

        public void OnLoad() {        }        public void OnEnable() {            UrlMapper.RegisterPluginPage("api/start", this);
            UrlMapper.RegisterPluginPage("api/stop", this);
            UrlMapper.RegisterPluginPage("api/restart", this);
            UrlMapper.RegisterPluginPage("api/kill", this);
            UrlMapper.RegisterPluginPage("api/getlog", this);
            UrlMapper.RegisterPluginPage("api/getstats", this);
        }        public void OnDisable() {        }        public bool OnHttpPageRequest(HtcHttpContext httpContext, string filename) {                        return false;        }        public bool OnHttpExtensionRequest(HtcHttpContext httpContext, string filename, string extension) {            return false;        }

    }
}
