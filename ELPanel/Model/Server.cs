using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ELPanel.Model {
	public class Server {

		public readonly ServerInfo ServerInfo;
		private Process _serverProcess;
		private readonly ProcessStartInfo _serverStartInfo;
		private readonly List<string> _serverLog;
		private readonly string _serverPidPath;

        public int PlayersOnline { get; private set; }
        public int MaxPlayers { get; private set; }
		public bool Active { get; private set; }

		public Server(ServerInfo serverInfo) {
			this.ServerInfo = serverInfo;
			var arguments = serverInfo.Arguments
				.Replace("%MinMemory%", serverInfo.MinMemory.ToString())
				.Replace("%MaxMemory%", serverInfo.MaxMemory.ToString());
			_serverStartInfo = new ProcessStartInfo() {
				FileName = serverInfo.FileName,
				WorkingDirectory = serverInfo.WorkingDirectory,
				Arguments = arguments,
				RedirectStandardError = true,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
			};
			_serverLog = new List<string>();
			_serverPidPath = Path.Combine(serverInfo.WorkingDirectory, "Process.pid");
		}

		private void ServerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e) {
			lock(_serverLog) {
				if (_serverLog.Count == 100) _serverLog.RemoveAt(99);
				_serverLog.Insert(0, e.Data);
			}
		}

		private void ServerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e) {
			lock (_serverLog) {
				if (_serverLog.Count == 100) _serverLog.RemoveAt(99);
				_serverLog.Insert(0, e.Data);
			}
		}

		public async Task Start() {
			_serverProcess = new Process {
				StartInfo = _serverStartInfo
			};
			_serverProcess.ErrorDataReceived += ServerProcess_ErrorDataReceived;
			_serverProcess.OutputDataReceived += ServerProcess_OutputDataReceived;
			_serverProcess.Start();
			if (File.Exists(_serverPidPath)) File.Delete(_serverPidPath);
            await File.WriteAllTextAsync(_serverPidPath, $"{_serverProcess.Id}");
        }

		public async Task Stop() {
			if (_serverProcess == null) return;
			SendCommand("stop");
			while (!_serverProcess.HasExited) {
				await Task.Delay(100);
			}
			if (File.Exists(_serverPidPath)) File.Delete(_serverPidPath);
		}

		public async Task Restart() {
			if (_serverProcess == null) return;
			await Stop();
			await Start();
		}

		public async Task Kill() {
			if (_serverProcess == null) return;
			_serverProcess.Kill();
			while (!_serverProcess.HasExited) {
				await Task.Delay(100);
			}
			if (File.Exists(_serverPidPath)) File.Delete(_serverPidPath);
		}

		public void CheckStatus() {
            if (_serverProcess != null) return;
            if (!File.Exists(_serverPidPath)) return;
            string data = File.ReadAllText(_serverPidPath);
            if (!int.TryParse(data, out int pid)) return;
            _serverProcess = Process.GetProcessById(pid);
            if(_serverProcess == null) {
                File.Delete(_serverPidPath);
                return;
            }
            _serverProcess.ErrorDataReceived += ServerProcess_ErrorDataReceived;
            _serverProcess.OutputDataReceived += ServerProcess_OutputDataReceived;
        }

		public void SendCommand(string command) {
			if (_serverProcess == null) return;
			_serverProcess.StandardInput.WriteLine(command);
		}

		public string GetLog() {
			string log = string.Empty;
			foreach(string data in _serverLog) {
				log += data + Environment.NewLine;
			}
			return log;
		}
	}
}
