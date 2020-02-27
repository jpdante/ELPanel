using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELPanel.Manager {
	public class ServerManager {

		private readonly Dictionary<int, Server> _servers;

		public ServerManager() {
			_servers = new Dictionary<int, Server>();
		}

		public async Task LoadServers() {
			_servers.Clear();
			var connection = await HtcPlugin.MySqlManager.GetMySqlConnection();
			ServerInfo[] serverInfos = await HtcPlugin.MySqlManager.GetServers(connection);
			await HtcPlugin.MySqlManager.CloseMySqlConnection(connection);
			foreach(var serverInfo in serverInfos) {
				_servers.Add(serverInfo.ServerID, new Server(serverInfo));
			}
		}

		public Server GetServer(int serverID) {
            return _servers.TryGetValue(serverID, out var value) ? value : null;
        }
	}
}
