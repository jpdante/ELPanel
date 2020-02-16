using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using ELPanel.Model;

namespace ELPanel.Manager {
    public class SessionManager {

        private readonly Dictionary<string, Session> _sessions;
        private readonly Timer _timer;

        public SessionManager() {
            _sessions = new Dictionary<string, Session>();
            _timer = new Timer {
                Interval = 1000
            };
            _timer.Elapsed += TimerOnElapsed;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e) {
            foreach (var value in _sessions.Values.ToArray()) {
                if (value.IsExpired()) {
                    _sessions.Remove(value.Id);
                }
            }
        }

        public void Start() {
            _timer.Start();
        }

        public void Stop() {
            _timer.Stop();
        }

        public void AddSession(string id) {
            _sessions.Add(id, new Session(id));
        }

        public bool HasSession(string id) {
            return _sessions.ContainsKey(id);
        }

        public void RemoveSession(string id) {
            _sessions.Remove(id);
        }

        public Session GetSession(string id) {
            return _sessions.ContainsKey(id) ? _sessions[id] : null;
        }
    }
}
