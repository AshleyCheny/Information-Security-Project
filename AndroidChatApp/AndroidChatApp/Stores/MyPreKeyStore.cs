using System;
using libsignal.state;
using System.Collections.Generic;

namespace AndroidChatApp.Activities
{
    internal class MyPreKeyStore : PreKeyStore
    {
        List<KeyValuePair<uint, PreKeyRecord>> PreKeyStore;

        public bool ContainsPreKey(uint preKeyId)
        {
            return PreKeyStore.Exists(x => x.Value.getId() == preKeyId);
        }

        public PreKeyRecord LoadPreKey(uint preKeyId)
        {
            return PreKeyStore.Find(x => x.Value.getId() == preKeyId).Value;
        }

        public void RemovePreKey(uint preKeyId)
        {
            PreKeyStore.RemoveAll(x => x.Value.getId() == preKeyId);
        }

        public void StorePreKey(uint preKeyId, PreKeyRecord record)
        {
            PreKeyStore.Add(new KeyValuePair<uint, PreKeyRecord>(preKeyId, record));
        }
    }
}