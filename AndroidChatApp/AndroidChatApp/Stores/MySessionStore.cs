using System;
using System.Collections.Generic;
using libsignal;
using libsignal.state;
using System.Linq;

namespace AndroidChatApp.Activities
{
    internal class MySessionStore : SessionStore
    {
        List<KeyValuePair<SignalProtocolAddress, SessionRecord>> SessionStore;

        public bool ContainsSession(SignalProtocolAddress address)
        {
            return SessionStore.Exists(x => x.Key == address);
        }

        public void DeleteAllSessions(string name)
        {
            SessionStore.RemoveAll(x => x.Key.getName() == name);
        }

        public void DeleteSession(SignalProtocolAddress address)
        {
            SessionStore.RemoveAll(x => x.Key == address);
        }

        public List<uint> GetSubDeviceSessions(string name)
        {
            return (from kvp in SessionStore where kvp.Key.getName() == name select kvp.Key.getDeviceId()).ToList();
        }

        public SessionRecord LoadSession(SignalProtocolAddress address)
        {
            return SessionStore.Find(x => x.Key == address).Value;
        }

        public void StoreSession(SignalProtocolAddress address, SessionRecord record)
        {
            SessionStore.Add(new KeyValuePair<SignalProtocolAddress, SessionRecord>(address, record));
        }
    }
}