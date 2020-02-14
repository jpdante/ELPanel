using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ELPanel {
	public class Server {

		private ServerInfo serverInfo;
		private Process serverProcess;
		private ProcessStartInfo serverStartInfo;
		private List<string> serverLog;
		private string serverPidPath;

		public Server(ServerInfo serverInfo) {
			this.serverInfo = serverInfo;
			var arguments = serverInfo.Arguments
				.Replace("%MinMemory%", serverInfo.MinMemory.ToString())
				.Replace("%MaxMemory%", serverInfo.MaxMemory.ToString());
			serverStartInfo = new ProcessStartInfo() {
				FileName = serverInfo.FileName,
				WorkingDirectory = serverInfo.WorkingDirectory,
				Arguments = arguments,
				RedirectStandardError = true,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
			};
			serverLog = new List<string>();
			serverPidPath = Path.Combine(serverInfo.WorkingDirectory, "Process.pid");
		}

		private void ServerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e) {
			lock(serverLog) {
				if (serverLog.Count == 100) serverLog.RemoveAt(99);
				serverLog.Insert(0, e.Data);
			}
		}

		private void ServerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e) {
			lock (serverLog) {
				if (serverLog.Count == 100) serverLog.RemoveAt(99);
				serverLog.Insert(0, e.Data);
			}
		}

		public async Task Start() {
			serverProcess = new Process {
				StartInfo = serverStartInfo
			};
			serverProcess.ErrorDataReceived += ServerProcess_ErrorDataReceived;
			serverProcess.OutputDataReceived += ServerProcess_OutputDataReceived;
			serverProcess.Start();
			if (File.Exists(serverPidPath)) File.Delete(serverPidPath);
			File.WriteAllText(serverPidPath, $"{serverProcess.Id}");
		}

		public async Task Stop() {
			if (serverProcess == null) return;
			SendCommand("stop");
			while (!serverProcess.HasExited) {
				await Task.Delay(100);
			}
			if (File.Exists(serverPidPath)) File.Delete(serverPidPath);
		}

		public async Task Restart() {
			if (serverProcess == null) return;
			await Stop();
			await Start();
		}

		public async Task Kill() {
			if (serverProcess == null) return;
			serverProcess.Kill();
			while (!serverProcess.HasExited) {
				await Task.Delay(100);
			}
			if (File.Exists(serverPidPath)) File.Delete(serverPidPath);
		}

		public void CheckStatus() {
			if (serverProcess == null) {
				if (File.Exists(serverPidPath)) {
					var data = File.ReadAllText(serverPidPath);
					if(int.TryParse(data, out int pid)) {
						serverProcess = Process.GetProcessById(pid);
						if(serverProcess == null) {
							File.Delete(serverPidPath);
							return;
						}
						serverProcess.ErrorDataReceived += ServerProcess_ErrorDataReceived;
						serverProcess.OutputDataReceived += ServerProcess_OutputDataReceived;
					}
				}
			}
		}

		public void SendCommand(string command) {
			if (serverProcess == null) return;
			serverProcess.StandardInput.WriteLine(command);
		}

		public string GetLog() {
			string log = string.Empty;
			foreach(string data in serverLog) {
				log += data + Environment.NewLine;
			}
			return log;
		}
	}
}
