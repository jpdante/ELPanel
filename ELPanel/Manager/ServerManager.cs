using System;
using System.Collections.Generic;

namespace ELPanel.Manager {
	public class ServerManager {

		private Dictionary<int, Server> servers;

		public ServerManager() {
			servers = new Dictionary<int, Server>();
		}

		public void LoadServers() {
			servers.Clear();
			var connection = HtcPlugin.MySqlManager.GetMySqlConnection();
			var serverInfos = HtcPlugin.MySqlManager.GetServers(connection);
			HtcPlugin.MySqlManager.CloseMySqlConnection(connection);
			foreach(var serverInfo in serverInfos) {
				servers.Add(serverInfo.ServerID, new Server(serverInfo));
			}
		}

		public Server GetServer(int serverID) {
			if(servers.TryGetValue(serverID, out var value)) {
				return value;
			}
			return null;
		}
	}
}
