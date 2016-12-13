/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
using System;
using System.Collections.Generic;

namespace libsignal.state.impl
{
    public class InMemoryPreKeyStore : PreKeyStore
	{

		public readonly IDictionary<uint, byte[]> store = new Dictionary<uint, byte[]>();


		public PreKeyRecord LoadPreKey(uint preKeyId)
		{
			try
			{
				if (!store.ContainsKey(preKeyId))
				{
					throw new InvalidKeyIdException("No such prekeyrecord!");
				}
				byte[] record;
				store.TryGetValue(preKeyId, out record);

				return new PreKeyRecord(record);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}


		public void StorePreKey(uint preKeyId, PreKeyRecord record)
		{
			store[preKeyId] = record.serialize();
		}


		public bool ContainsPreKey(uint preKeyId)
		{
			return store.ContainsKey(preKeyId);
		}


		public void RemovePreKey(uint preKeyId)
		{
			store.Remove(preKeyId);
		}
	}
}
