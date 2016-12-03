using System;
using System.Collections.Generic;
using libsignal.state;
using libsignal;
using System.Linq;

namespace AndroidChatApp.Activities
{
    internal class MySignedPreKeyStore : SignedPreKeyStore
    {
        List<KeyValuePair<uint, SignedPreKeyRecord>> SignedPreKeyStore;

        public bool ContainsSignedPreKey(uint signedPreKeyId)
        {
            return SignedPreKeyStore.Exists(x => x.Key == signedPreKeyId);
        }

        public SignedPreKeyRecord LoadSignedPreKey(uint signedPreKeyId)
        {
            try
            {
                return SignedPreKeyStore.Find(x => x.Key == signedPreKeyId).Value;
            }
            catch (Exception e)
            {
                throw new InvalidKeyIdException(e);
            }
        }

        public List<SignedPreKeyRecord> LoadSignedPreKeys()
        {
            return (from kvp in SignedPreKeyStore select kvp.Value).Distinct().ToList();
        }

        public void RemoveSignedPreKey(uint signedPreKeyId)
        {
            SignedPreKeyStore.RemoveAll(x => x.Key == signedPreKeyId);
        }

        public void StoreSignedPreKey(uint signedPreKeyId, SignedPreKeyRecord record)
        {
            SignedPreKeyStore.Add(new KeyValuePair<uint, SignedPreKeyRecord>(signedPreKeyId, record));
        }
    }
}