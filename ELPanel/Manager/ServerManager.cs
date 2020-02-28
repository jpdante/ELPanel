using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Timers;
using ELPanel.Model;
using HtcSharp.Core.Logging.Abstractions;

namespace ELPanel.Manager {
	public class ServerManager {

		private readonly Dictionary<int, Server> _servers;
        private readonly Timer _timer;

		public ServerManager() {
			_servers = new Dictionary<int, Server>();
            _timer = new Timer {
                Interval = 1000
            };
            _timer.Elapsed += TimerOnElapsed;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e) {
            foreach (var server in _servers.Values) {
                server.CheckStatus();
            }
        }

        public void Start() {
            _timer.Start();
        }

        public void Stop() {
            _timer.Stop();
        }

		public async Task LoadServers() {
			_servers.Clear();
			var connection = await HtcPlugin.MySqlManager.GetMySqlConnection();
			ServerInfo[] serverInfos = await HtcPlugin.MySqlManager.GetServers(connection);
			await HtcPlugin.MySqlManager.CloseMySqlConnection(connection);
			foreach(var serverInfo in serverInfos) {
				_servers.Add(serverInfo.ServerID, new Server(serverInfo));
			}
            foreach (var server in _servers.Values) {
                server.CheckStatus();
            }
		}

		public Server GetServer(int serverId) {
            return _servers.TryGetValue(serverId, out var value) ? value : null;
        }

        public ImmutableArray<Server> GetServers() {
            return _servers.Values.ToImmutableArray();

        }
	}
}
