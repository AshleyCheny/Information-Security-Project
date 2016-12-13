﻿/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
 using Google.ProtocolBuffers;
using libsignal.ecc;
using libsignal.kdf;
using libsignal.ratchet;
using libsignal.util;
using Strilanc.Value;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static libsignal.state.StorageProtos;
using static libsignal.state.StorageProtos.SessionStructure.Types;

namespace libsignal.state
{
    public class SessionState
	{
		private static readonly int MAX_MESSAGE_KEYS = 2000;

		private SessionStructure sessionStructure;

		public SessionState()
		{
            sessionStructure = SessionStructure.CreateBuilder().Build();
		}

		public SessionState(SessionStructure sessionStructure)
		{
			this.sessionStructure = sessionStructure;
		}

		public SessionState(SessionState copy)
		{
            sessionStructure = copy.sessionStructure.ToBuilder().Build();
		}

		public SessionStructure getStructure()
		{
			return sessionStructure;
		}

		public byte[] getAliceBaseKey()
		{
			return sessionStructure.AliceBaseKey.ToByteArray();
		}

		public void setAliceBaseKey(byte[] aliceBaseKey)
		{
            sessionStructure = sessionStructure.ToBuilder()
														 .SetAliceBaseKey(ByteString.CopyFrom(aliceBaseKey))
														 .Build();
		}

		public void setSessionVersion(uint version)
		{
            sessionStructure = sessionStructure.ToBuilder()
														 .SetSessionVersion(version)
														 .Build();
		}

		public uint getSessionVersion()
		{
			uint sessionVersion = sessionStructure.SessionVersion;

			if (sessionVersion == 0) return 2;
			else return sessionVersion;
		}

		public void setRemoteIdentityKey(IdentityKey identityKey)
		{
            sessionStructure = sessionStructure.ToBuilder()
														 .SetRemoteIdentityPublic(ByteString.CopyFrom(identityKey.serialize()))
														 .Build();
		}

		public void setLocalIdentityKey(IdentityKey identityKey)
		{
            sessionStructure = sessionStructure.ToBuilder()
														 .SetLocalIdentityPublic(ByteString.CopyFrom(identityKey.serialize()))
														 .Build();
		}

		public IdentityKey getRemoteIdentityKey()
		{
			try
			{
				if (!sessionStructure.HasRemoteIdentityPublic)
				{
					return null;
				}

				return new IdentityKey(sessionStructure.RemoteIdentityPublic.ToByteArray(), 0);
			}
			catch (InvalidKeyException e)
			{
				Debug.WriteLine(e.ToString(), "SessionRecordV2");
				return null;
			}
		}

		public IdentityKey getLocalIdentityKey()
		{
			try
			{
				return new IdentityKey(sessionStructure.LocalIdentityPublic.ToByteArray(), 0);
			}
			catch (InvalidKeyException e)
			{
				throw new Exception(e.Message);
			}
		}

		public uint getPreviousCounter()
		{
			return sessionStructure.PreviousCounter;
		}

		public void setPreviousCounter(uint previousCounter)
		{
            sessionStructure = sessionStructure.ToBuilder()
														 .SetPreviousCounter(previousCounter)
														 .Build();
		}

		public RootKey getRootKey()
		{
			return new RootKey(HKDF.createFor(getSessionVersion()),
                               sessionStructure.RootKey.ToByteArray());
		}

		public void setRootKey(RootKey rootKey)
		{
            sessionStructure = sessionStructure.ToBuilder()
														 .SetRootKey(ByteString.CopyFrom(rootKey.getKeyBytes()))
														 .Build();
		}

		public ECPublicKey getSenderRatchetKey()
		{
			try
			{
				return Curve.decodePoint(sessionStructure.SenderChain.SenderRatchetKey.ToByteArray(), 0);
			}
			catch (InvalidKeyException e)
			{
				throw new Exception(e.Message);
			}
		}

		public ECKeyPair getSenderRatchetKeyPair()
		{
			ECPublicKey publicKey = getSenderRatchetKey();
			ECPrivateKey privateKey = Curve.decodePrivatePoint(sessionStructure.SenderChain
																			   .SenderRatchetKeyPrivate
																			   .ToByteArray());

			return new ECKeyPair(publicKey, privateKey);
		}

		public bool hasReceiverChain(ECPublicKey senderEphemeral)
		{
			return getReceiverChain(senderEphemeral) != null;
		}

		public bool hasSenderChain()
		{
			return sessionStructure.HasSenderChain;
		}

		private Pair<Chain, uint> getReceiverChain(ECPublicKey senderEphemeral)
		{
			IList<Chain> receiverChains = sessionStructure.ReceiverChainsList;
			uint index = 0;

			foreach (Chain receiverChain in receiverChains)
			{
				try
				{
					ECPublicKey chainSenderRatchetKey = Curve.decodePoint(receiverChain.SenderRatchetKey.ToByteArray(), 0);

					if (chainSenderRatchetKey.Equals(senderEphemeral))
					{
						return new Pair<Chain, uint>(receiverChain, index);
					}
				}
				catch (InvalidKeyException e)
				{
					Debug.WriteLine(e.ToString(), "SessionRecordV2");
				}

				index++;
			}

			return null;
		}

		public ChainKey getReceiverChainKey(ECPublicKey senderEphemeral)
		{
			Pair<Chain, uint> receiverChainAndIndex = getReceiverChain(senderEphemeral);
			Chain receiverChain = receiverChainAndIndex.first();

			if (receiverChain == null)
			{
				return null;
			}
			else
			{
				return new ChainKey(HKDF.createFor(getSessionVersion()),
									receiverChain.ChainKey.Key.ToByteArray(),
									receiverChain.ChainKey.Index);
			}
		}

		public void addReceiverChain(ECPublicKey senderRatchetKey, ChainKey chainKey)
		{
			Chain.Types.ChainKey chainKeyStructure = Chain.Types.ChainKey.CreateBuilder()
															 .SetKey(ByteString.CopyFrom(chainKey.getKey()))
															 .SetIndex(chainKey.getIndex())
															 .Build();

			Chain chain = Chain.CreateBuilder()
							   .SetChainKey(chainKeyStructure)
							   .SetSenderRatchetKey(ByteString.CopyFrom(senderRatchetKey.serialize()))
							   .Build();

            sessionStructure = sessionStructure.ToBuilder().AddReceiverChains(chain).Build();

			if (sessionStructure.ReceiverChainsList.Count > 5)
			{
                sessionStructure = sessionStructure.ToBuilder().Build();
			}
		}

		public void setSenderChain(ECKeyPair senderRatchetKeyPair, ChainKey chainKey)
		{
			Chain.Types.ChainKey chainKeyStructure = Chain.Types.ChainKey.CreateBuilder()
															 .SetKey(ByteString.CopyFrom(chainKey.getKey()))
															 .SetIndex(chainKey.getIndex())
															 .Build();

			Chain senderChain = Chain.CreateBuilder()
									 .SetSenderRatchetKey(ByteString.CopyFrom(senderRatchetKeyPair.getPublicKey().serialize()))
									 .SetSenderRatchetKeyPrivate(ByteString.CopyFrom(senderRatchetKeyPair.getPrivateKey().serialize()))
									 .SetChainKey(chainKeyStructure)
									 .Build();

            sessionStructure = sessionStructure.ToBuilder().SetSenderChain(senderChain).Build();
		}

		public ChainKey getSenderChainKey()
		{
			Chain.Types.ChainKey chainKeyStructure = sessionStructure.SenderChain.ChainKey;
			return new ChainKey(HKDF.createFor(getSessionVersion()),
								chainKeyStructure.Key.ToByteArray(), chainKeyStructure.Index);
		}


		public void setSenderChainKey(ChainKey nextChainKey)
		{
			Chain.Types.ChainKey chainKey = Chain.Types.ChainKey.CreateBuilder()
													.SetKey(ByteString.CopyFrom(nextChainKey.getKey()))
													.SetIndex(nextChainKey.getIndex())
													.Build();

			Chain chain = sessionStructure.SenderChain.ToBuilder()
										  .SetChainKey(chainKey).Build();

            sessionStructure = sessionStructure.ToBuilder().SetSenderChain(chain).Build();
		}

		public bool hasMessageKeys(ECPublicKey senderEphemeral, uint counter)
		{
			Pair<Chain, uint> chainAndIndex = getReceiverChain(senderEphemeral);
			Chain chain = chainAndIndex.first();

			if (chain == null)
			{
				return false;
			}

			IList<Chain.Types.MessageKey> messageKeyList = chain.MessageKeysList;

			foreach (Chain.Types.MessageKey messageKey in messageKeyList)
			{
				if (messageKey.Index == counter)
				{
					return true;
				}
			}

			return false;
		}

		public MessageKeys removeMessageKeys(ECPublicKey senderEphemeral, uint counter)
		{
			Pair<Chain, uint> chainAndIndex = getReceiverChain(senderEphemeral);
			Chain chain = chainAndIndex.first();

			if (chain == null)
			{
				return null;
			}

			List<Chain.Types.MessageKey> messageKeyList = new List<Chain.Types.MessageKey>(chain.MessageKeysList);
			IEnumerator<Chain.Types.MessageKey> messageKeyIterator = messageKeyList.GetEnumerator();
			MessageKeys result = null;

			while (messageKeyIterator.MoveNext())
			{
				Chain.Types.MessageKey messageKey = messageKeyIterator.Current;

				if (messageKey.Index == counter)
				{
					result = new MessageKeys(messageKey.CipherKey.ToByteArray(),
											messageKey.MacKey.ToByteArray(),
											 messageKey.Iv.ToByteArray(),
											 messageKey.Index);

					messageKeyList.Remove(messageKey);
					break;
				}
			}

			Chain updatedChain = chain.ToBuilder().ClearMessageKeys()
									  .AddRangeMessageKeys(messageKeyList)
									  .Build();

            sessionStructure = sessionStructure.ToBuilder()
														 .SetReceiverChains((int)chainAndIndex.second(), updatedChain)
														 .Build();

			return result;
		}

		public void setMessageKeys(ECPublicKey senderEphemeral, MessageKeys messageKeys)
		{
			Pair<Chain, uint> chainAndIndex = getReceiverChain(senderEphemeral);
			Chain chain = chainAndIndex.first();
			Chain.Types.MessageKey messageKeyStructure = Chain.Types.MessageKey.CreateBuilder()
																	  .SetCipherKey(ByteString.CopyFrom(messageKeys.getCipherKey()))
																	  .SetMacKey(ByteString.CopyFrom(messageKeys.getMacKey()))
																	  .SetIndex(messageKeys.getCounter())
																	  .SetIv(ByteString.CopyFrom(messageKeys.getIv()))
																	  .Build();

			Chain.Builder updatedChain = chain.ToBuilder().AddMessageKeys(messageKeyStructure);
			if (updatedChain.MessageKeysList.Count > MAX_MESSAGE_KEYS)
			{
				updatedChain.MessageKeysList.RemoveAt(0);
			}

            sessionStructure = sessionStructure.ToBuilder()
														 .SetReceiverChains((int)chainAndIndex.second(), updatedChain.Build())
														 .Build();
		}

		public void setReceiverChainKey(ECPublicKey senderEphemeral, ChainKey chainKey)
		{
			Pair<Chain, uint> chainAndIndex = getReceiverChain(senderEphemeral);
			Chain chain = chainAndIndex.first();

			Chain.Types.ChainKey chainKeyStructure = Chain.Types.ChainKey.CreateBuilder()
															 .SetKey(ByteString.CopyFrom(chainKey.getKey()))
															 .SetIndex(chainKey.getIndex())
															 .Build();

			Chain updatedChain = chain.ToBuilder().SetChainKey(chainKeyStructure).Build();

            sessionStructure = sessionStructure.ToBuilder()
														 .SetReceiverChains((int)chainAndIndex.second(), updatedChain)
														 .Build();
		}

		public void setPendingKeyExchange(uint sequence,
										  ECKeyPair ourBaseKey,
										  ECKeyPair ourRatchetKey,
										  IdentityKeyPair ourIdentityKey)
		{
			PendingKeyExchange structure =
				PendingKeyExchange.CreateBuilder()
								  .SetSequence(sequence)
								  .SetLocalBaseKey(ByteString.CopyFrom(ourBaseKey.getPublicKey().serialize()))
								  .SetLocalBaseKeyPrivate(ByteString.CopyFrom(ourBaseKey.getPrivateKey().serialize()))
								  .SetLocalRatchetKey(ByteString.CopyFrom(ourRatchetKey.getPublicKey().serialize()))
								  .SetLocalRatchetKeyPrivate(ByteString.CopyFrom(ourRatchetKey.getPrivateKey().serialize()))
								  .SetLocalIdentityKey(ByteString.CopyFrom(ourIdentityKey.getPublicKey().serialize()))
								  .SetLocalIdentityKeyPrivate(ByteString.CopyFrom(ourIdentityKey.getPrivateKey().serialize()))
								  .Build();

            sessionStructure = sessionStructure.ToBuilder()
														 .SetPendingKeyExchange(structure)
														 .Build();
		}

		public uint getPendingKeyExchangeSequence()
		{
			return sessionStructure.PendingKeyExchange.Sequence;
		}

		public ECKeyPair getPendingKeyExchangeBaseKey()
		{
			ECPublicKey publicKey = Curve.decodePoint(sessionStructure.PendingKeyExchange
																.LocalBaseKey.ToByteArray(), 0);

			ECPrivateKey privateKey = Curve.decodePrivatePoint(sessionStructure.PendingKeyExchange
																	   .LocalBaseKeyPrivate
																	   .ToByteArray());

			return new ECKeyPair(publicKey, privateKey);
		}

		public ECKeyPair getPendingKeyExchangeRatchetKey()
		{
			ECPublicKey publicKey = Curve.decodePoint(sessionStructure.PendingKeyExchange
																.LocalRatchetKey.ToByteArray(), 0);

			ECPrivateKey privateKey = Curve.decodePrivatePoint(sessionStructure.PendingKeyExchange
																	   .LocalRatchetKeyPrivate
																	   .ToByteArray());

			return new ECKeyPair(publicKey, privateKey);
		}

		public IdentityKeyPair getPendingKeyExchangeIdentityKey()
		{
			IdentityKey publicKey = new IdentityKey(sessionStructure.PendingKeyExchange
															.LocalIdentityKey.ToByteArray(), 0);

			ECPrivateKey privateKey = Curve.decodePrivatePoint(sessionStructure.PendingKeyExchange
																	   .LocalIdentityKeyPrivate
																	   .ToByteArray());

			return new IdentityKeyPair(publicKey, privateKey);
		}

		public bool hasPendingKeyExchange()
		{
			return sessionStructure.HasPendingKeyExchange;
		}

		public void setUnacknowledgedPreKeyMessage(May<uint> preKeyId, uint signedPreKeyId, ECPublicKey baseKey)
		{
			PendingPreKey.Builder pending = PendingPreKey.CreateBuilder()
														 .SetSignedPreKeyId((int)signedPreKeyId)
														 .SetBaseKey(ByteString.CopyFrom(baseKey.serialize()));

			if (preKeyId.HasValue)
			{
				pending.SetPreKeyId(preKeyId.ForceGetValue());
			}

            sessionStructure = sessionStructure.ToBuilder()
														 .SetPendingPreKey(pending.Build())
														 .Build();
		}

		public bool hasUnacknowledgedPreKeyMessage()
		{
			return sessionStructure.HasPendingPreKey;
		}

		public UnacknowledgedPreKeyMessageItems getUnacknowledgedPreKeyMessageItems()
		{
			try
			{
				May<uint> preKeyId;

				if (sessionStructure.PendingPreKey.HasPreKeyId)
				{
					preKeyId = new May<uint>(sessionStructure.PendingPreKey.PreKeyId);
				}
				else
				{
					preKeyId = May<uint>.NoValue;
				}

				return
					new UnacknowledgedPreKeyMessageItems(preKeyId,
														 (uint)sessionStructure.PendingPreKey.SignedPreKeyId,
														 Curve.decodePoint(sessionStructure.PendingPreKey
																						   .BaseKey
																						   .ToByteArray(), 0));
			}
			catch (InvalidKeyException e)
			{
				throw new Exception(e.Message);
			}
		}

		public void clearUnacknowledgedPreKeyMessage()
		{
            sessionStructure = sessionStructure.ToBuilder()
														 .ClearPendingPreKey()
														 .Build();
		}

		public void setRemoteRegistrationId(uint registrationId)
		{
            sessionStructure = sessionStructure.ToBuilder()
														 .SetRemoteRegistrationId(registrationId)
														 .Build();
		}

		public uint getRemoteRegistrationId()
		{
			return sessionStructure.RemoteRegistrationId;
		}

		public void setLocalRegistrationId(uint registrationId)
		{
            sessionStructure = sessionStructure.ToBuilder()
														 .SetLocalRegistrationId(registrationId)
														 .Build();
		}

		public uint GetLocalRegistrationId()
		{
			return sessionStructure.LocalRegistrationId;
		}

		public byte[] serialize()
		{
			return sessionStructure.ToByteArray();
		}

		public class UnacknowledgedPreKeyMessageItems
		{
			private readonly May<uint> preKeyId;
			private readonly uint signedPreKeyId;
			private readonly ECPublicKey baseKey;

			public UnacknowledgedPreKeyMessageItems(May<uint> preKeyId,
													uint signedPreKeyId,
													ECPublicKey baseKey)
			{
				this.preKeyId = preKeyId;
				this.signedPreKeyId = signedPreKeyId;
				this.baseKey = baseKey;
			}


			public May<uint> getPreKeyId()
			{
				return preKeyId;
			}

			public uint getSignedPreKeyId()
			{
				return signedPreKeyId;
			}

			public ECPublicKey getBaseKey()
			{
				return baseKey;
			}
		}
	}
}
