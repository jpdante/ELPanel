using System;
namespace ELPanel {
	public class ServerInfo {

		public int ServerID { get; private set; }
		public string ServerName { get; private set; }
		public string FileName { get; private set; }
		public string WorkingDirectory { get; private set; }
		public string Arguments { get; private set; }
	    public int MinMemory { get; private set; }
	    public int MaxMemory { get; private set; }

	    public ServerInfo(int serverID, string serverName, string fileName, string workingDirectory, string arguments, int minMemory, int maxMemory) {
			this.ServerID = serverID;
			this.ServerName = serverName;
			this.FileName = fileName;
			this.WorkingDirectory = workingDirectory;
			this.Arguments = arguments;
			this.MinMemory = minMemory;
			this.MaxMemory = maxMemory;
    	}
    }
}