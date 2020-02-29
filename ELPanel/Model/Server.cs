using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;

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
			string arguments = serverInfo.Arguments
				.Replace("%MinMemory%", serverInfo.MinMemory.ToString())
				.Replace("%MaxMemory%", serverInfo.MaxMemory.ToString())
                .Replace("%FileName%", serverInfo.FileName);
			_serverStartInfo = new ProcessStartInfo() {
				FileName = @"/usr/bin/java",
				WorkingDirectory = serverInfo.WorkingDirectory,
				Arguments = arguments,
				RedirectStandardError = true,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
			};
			_serverLog = new List<string>();
			_serverPidPath = Path.Combine(serverInfo.WorkingDirectory, "process.pid");
		}

		private void ServerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e) {
			lock(_serverLog) {
				if (_serverLog.Count == 50) _serverLog.RemoveAt(49);
				_serverLog.Insert(0, e.Data);
            }
		}

		private void ServerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e) {
			lock (_serverLog) {
				if (_serverLog.Count == 50) _serverLog.RemoveAt(49);
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
            Active = true;
			_serverProcess.BeginOutputReadLine();
            _serverProcess.BeginErrorReadLine();
			if (File.Exists(_serverPidPath)) File.Delete(_serverPidPath);
            await File.WriteAllTextAsync(_serverPidPath, $"{_serverProcess.Id}");
        }

		public async Task Stop() {
			if (_serverProcess == null) return;
			await SendCommand("stop");
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
			_serverProcess.Kill(true);
			while (!_serverProcess.HasExited) {
				await Task.Delay(100);
			}
			if (File.Exists(_serverPidPath)) File.Delete(_serverPidPath);
		}

		public void CheckStatus() {
            if (_serverProcess != null) {
                Active = !_serverProcess.HasExited;
                return;
            }
            if (!File.Exists(_serverPidPath)) return;
            string data = File.ReadAllText(_serverPidPath);
            if (!int.TryParse(data, out int pid)) return;
            try {
                var process = Process.GetProcessById(pid);
                process.Kill(true);
            } catch {
                // ignored
            }
            if (_serverProcess != null) return;
            File.Delete(_serverPidPath);
        }

		public async Task SendCommand(string command) {
            if (_serverProcess == null) return;
            await _serverProcess.StandardInput.WriteAsync($"{command}{Environment.NewLine}");
		}

		public string GetLog() {
            lock (_serverLog) {
                return _serverLog.Aggregate(string.Empty, (current, data) => current + (data + Environment.NewLine));
            }
        }
	}
}
