using System;

namespace ELPanel.Model {
    public class Session {

        public string Id { get; }
        public int Expiration { get; private set; }

        public Session(string id) {
            Id = id;
            Expiration = 3600;
        }

        public bool IsExpired() {
            Expiration--;
            return Expiration <= 0;
        }
    }
}