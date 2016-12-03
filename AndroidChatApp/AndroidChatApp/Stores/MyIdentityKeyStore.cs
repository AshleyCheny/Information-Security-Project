using System;
using libsignal;
using libsignal.state;
using Android.Preferences;
using Android.Content;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AndroidChatApp.Activities
{
    internal class MyIdentityKeyStore : LoginActivity, IdentityKeyStore
    {
        List<KeyValuePair<string, IdentityKey>> IdentityKeyStore;

        public IdentityKeyPair GetIdentityKeyPair()
        {
            ISharedPreferences sharedprefs = PreferenceManager.GetDefaultSharedPreferences(this);
            byte[] IdentityKeyPairBytes = JsonConvert.DeserializeObject<byte[]>(sharedprefs.GetString("IdentityKeyPair", string.Empty));
            IdentityKeyPair insertKeyPair = new IdentityKeyPair(IdentityKeyPairBytes);
            string username = sharedprefs.GetString("Username", string.Empty);
            IdentityKeyStore.Add(new KeyValuePair<string, IdentityKey>(username, insertKeyPair.getPublicKey()));
            return insertKeyPair;
        }

        public uint GetLocalRegistrationId()
        {
            ISharedPreferences sharedprefs = PreferenceManager.GetDefaultSharedPreferences(this);
            return Convert.ToUInt32(sharedprefs.GetString("RegistrationId", string.Empty));
        }

        public bool IsTrustedIdentity(string name, IdentityKey identityKey)
        {
            return IdentityKeyStore.Contains(new KeyValuePair<string, IdentityKey>(name, identityKey));
        }

        public bool SaveIdentity(string name, IdentityKey identityKey)
        {
            try
            {
                IdentityKeyStore.Add(new KeyValuePair<string, IdentityKey>(name, identityKey));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}