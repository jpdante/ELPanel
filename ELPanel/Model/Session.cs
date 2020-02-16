using System;

namespace ELPanel.Model {
    public class Session {

        public string Id { get; }
        private int _expiration;

        public Session(string id) {
            Id = id;
            _expiration = 3600;
        }

        public bool IsExpired() {
            _expiration--;
            return _expiration <= 0;
        }
    }
}