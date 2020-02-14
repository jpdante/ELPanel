using System;
using System.Collections.Generic;
using ELPanel.Model;

namespace ELPanel.Manager {
	public class SessionManager {

		private Dictionary<string, Session> sessions;

		public SessionManager() {
			sessions = new Dictionary<string, Session>();
		}

		public void AddSession(string id) {
			sessions.Add(id, new Session());
		}

		public void RemoveSession(string id) {
			sessions.Remove(id);
		}

		public Session GetSession(string id) {
			if (sessions.ContainsKey(id)) return sessions[id];
			var connection = HtcPlugin.MySqlManager.GetMySqlConnection();
			if (HtcPlugin.MySqlManager.HasSession(id, connection)) {
				AddSession(id);
			}
			HtcPlugin.MySqlManager.CloseMySqlConnection(connection);
			return sessions[id];
		}
	}
}
