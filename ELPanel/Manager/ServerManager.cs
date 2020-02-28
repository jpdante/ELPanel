﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using ELPanel.Model;

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

		public Server GetServer(int serverId) {
            return _servers.TryGetValue(serverId, out var value) ? value : null;
        }

        public ImmutableArray<Server> GetServers() {
            return _servers.Values.ToImmutableArray();

        }
	}
}
