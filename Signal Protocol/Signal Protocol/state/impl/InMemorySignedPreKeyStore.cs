/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
 using System;
using System.Collections.Generic;

namespace libsignal.state.impl
{
    public class InMemorySignedPreKeyStore : SignedPreKeyStore
	{

		public readonly IDictionary<uint, byte[]> store = new Dictionary<uint, byte[]>();


		public SignedPreKeyRecord LoadSignedPreKey(uint signedPreKeyId)
		{
			try
			{
				if (!store.ContainsKey(signedPreKeyId))
				{
					throw new InvalidKeyIdException("No such signedprekeyrecord! " + signedPreKeyId);
				}

				byte[] record;
				store.TryGetValue(signedPreKeyId, out record);

				return new SignedPreKeyRecord(record);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}


		public List<SignedPreKeyRecord> LoadSignedPreKeys()
		{
			try
			{
				List<SignedPreKeyRecord> results = new List<SignedPreKeyRecord>();

				foreach (byte[] serialized in store.Values)
				{
					results.Add(new SignedPreKeyRecord(serialized));
				}

				return results;
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}


		public void StoreSignedPreKey(uint signedPreKeyId, SignedPreKeyRecord record)
		{
			store[signedPreKeyId] = record.serialize();
		}


		public bool ContainsSignedPreKey(uint signedPreKeyId)
		{
			return store.ContainsKey(signedPreKeyId);
		}


		public void RemoveSignedPreKey(uint signedPreKeyId)
		{
			store.Remove(signedPreKeyId);
		}
	}
}
