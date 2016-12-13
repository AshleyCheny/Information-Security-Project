/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
using System;
using System.Collections.Generic;

namespace libsignal.state.impl
{
	public class InMemoryIdentityKeyStore : IdentityKeyStore
	{

		private readonly IDictionary<string, IdentityKey> trustedKeys = new Dictionary<String, IdentityKey>();

		private IdentityKeyPair identityKeyPair;
		private uint localRegistrationId;
        
		public InMemoryIdentityKeyStore(IdentityKeyPair identityKeyPair, uint localRegistrationId)
		{
			this.identityKeyPair = identityKeyPair;
			this.localRegistrationId = localRegistrationId;
		}

		public IdentityKeyPair GetIdentityKeyPair()
		{
			return identityKeyPair;
		}

        public List<TrustedKey> GetAllTrustedKeys()
        {
            List<TrustedKey> OutTrustedKey = new List<TrustedKey>();
            foreach (KeyValuePair<string, IdentityKey> item in trustedKeys)
            {
                TrustedKey t = new TrustedKey();
                t.Name = item.Key;
                t.Identity = item.Value.serialize();
                OutTrustedKey.Add(t);
            }
            return OutTrustedKey;
        }

        public uint GetLocalRegistrationId()
		{
			return localRegistrationId;
		}

        public void PutValues(IdentityKeyPair identityKeyPair, uint localRegistrationId)
        {
            this.identityKeyPair = identityKeyPair;
            this.localRegistrationId = localRegistrationId;
        }

        public bool SaveIdentity(string name, IdentityKey identityKey)
		{
			trustedKeys[name] = identityKey;
			return true;
		}

		public bool IsTrustedIdentity(string name, IdentityKey identityKey)
		{
			IdentityKey trusted;
			trustedKeys.TryGetValue(name, out trusted);
			return (trusted == null || trusted.Equals(identityKey));
		}
	}
}
