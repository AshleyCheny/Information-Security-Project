/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
using libsignal.ecc;
using libsignal.protocol;
using libsignal.ratchet;
using libsignal.state;
using libsignal.util;
using Strilanc.Value;
using System;
using System.Collections.Generic;

namespace libsignal
{

    /**
     * The main entry point for Signal Protocol encrypt/decrypt operations.
     *
     * Once a session has been established with {@link SessionBuilder},
     * this class can be used for all encrypt/decrypt operations within
     * that session.
     *
     * @author Moxie Marlinspike
     */
    public class SessionCipher
	{

		public static readonly Object SESSION_LOCK = new Object();

		private readonly SessionStore sessionStore;
		private readonly SessionBuilder sessionBuilder;
		private readonly PreKeyStore preKeyStore;
		private readonly SignalProtocolAddress remoteAddress;

		/**
         * Construct a SessionCipher for encrypt/decrypt operations on a session.
         * In order to use SessionCipher, a session must have already been created
         * and stored using {@link SessionBuilder}.
         *
         * @param  sessionStore The {@link SessionStore} that contains a session for this recipient.
         * @param  remoteAddress  The remote address that messages will be encrypted to or decrypted from.
         */
		public SessionCipher(SessionStore sessionStore, PreKeyStore preKeyStore,
							 SignedPreKeyStore signedPreKeyStore, IdentityKeyStore identityKeyStore,
							 SignalProtocolAddress remoteAddress)
		{
			this.sessionStore = sessionStore;
			this.preKeyStore = preKeyStore;
			this.remoteAddress = remoteAddress;
            sessionBuilder = new SessionBuilder(sessionStore, preKeyStore, signedPreKeyStore,
													 identityKeyStore, remoteAddress);
		}

		public SessionCipher(SignalProtocolStore store, SignalProtocolAddress remoteAddress)
			: this(store, store, store, store, remoteAddress)
		{

		}

		/**
         * Encrypt a message.
         *
         * @param  paddedMessage The plaintext message bytes, optionally padded to a constant multiple.
         * @return A ciphertext message encrypted to the recipient+device tuple.
         */
		public CiphertextMessage encrypt(byte[] paddedMessage)
		{
			lock (SESSION_LOCK)
			{
				SessionRecord sessionRecord = sessionStore.LoadSession(remoteAddress);
				SessionState sessionState = sessionRecord.getSessionState();
				ChainKey chainKey = sessionState.getSenderChainKey();
				MessageKeys messageKeys = chainKey.getMessageKeys();
				ECPublicKey senderEphemeral = sessionState.getSenderRatchetKey();
				uint previousCounter = sessionState.getPreviousCounter();
				uint sessionVersion = sessionState.getSessionVersion();

				byte[] ciphertextBody = getCiphertext(sessionVersion, messageKeys, paddedMessage);
				CiphertextMessage ciphertextMessage = new SignalMessage(sessionVersion, messageKeys.getMacKey(),
																		 senderEphemeral, chainKey.getIndex(),
																		 previousCounter, ciphertextBody,
																		 sessionState.getLocalIdentityKey(),
																		 sessionState.getRemoteIdentityKey());

				if (sessionState.hasUnacknowledgedPreKeyMessage())
				{
					SessionState.UnacknowledgedPreKeyMessageItems items = sessionState.getUnacknowledgedPreKeyMessageItems();
					uint localRegistrationId = sessionState.GetLocalRegistrationId();

					ciphertextMessage = new PreKeySignalMessage(sessionVersion, localRegistrationId, items.getPreKeyId(),
																 items.getSignedPreKeyId(), items.getBaseKey(),
																 sessionState.getLocalIdentityKey(),
																 (SignalMessage)ciphertextMessage);
				}

				sessionState.setSenderChainKey(chainKey.getNextChainKey());
				sessionStore.StoreSession(remoteAddress, sessionRecord);
				return ciphertextMessage;
			}
		}

		/**
         * Decrypt a message.
         *
         * @param  ciphertext The {@link PreKeySignalMessage} to decrypt.
         *
         * @return The plaintext.
         * @throws InvalidMessageException if the input is not valid ciphertext.
         * @throws DuplicateMessageException if the input is a message that has already been received.
         * @throws LegacyMessageException if the input is a message formatted by a protocol version that
         *                                is no longer supported.
         * @throws InvalidKeyIdException when there is no local {@link org.whispersystems.libsignal.state.PreKeyRecord}
         *                               that corresponds to the PreKey ID in the message.
         * @throws InvalidKeyException when the message is formatted incorrectly.
         * @throws UntrustedIdentityException when the {@link IdentityKey} of the sender is untrusted.
         */
		public byte[] decrypt(PreKeySignalMessage ciphertext)

		{
			return decrypt(ciphertext, new NullDecryptionCallback());
		}

		/**
         * Decrypt a message.
         *
         * @param  ciphertext The {@link PreKeySignalMessage} to decrypt.
         * @param  callback   A callback that is triggered after decryption is complete,
         *                    but before the updated session state has been committed to the session
         *                    DB.  This allows some implementations to store the committed plaintext
         *                    to a DB first, in case they are concerned with a crash happening between
         *                    the time the session state is updated but before they're able to store
         *                    the plaintext to disk.
         *
         * @return The plaintext.
         * @throws InvalidMessageException if the input is not valid ciphertext.
         * @throws DuplicateMessageException if the input is a message that has already been received.
         * @throws LegacyMessageException if the input is a message formatted by a protocol version that
         *                                is no longer supported.
         * @throws InvalidKeyIdException when there is no local {@link org.whispersystems.libsignal.state.PreKeyRecord}
         *                               that corresponds to the PreKey ID in the message.
         * @throws InvalidKeyException when the message is formatted incorrectly.
         * @throws UntrustedIdentityException when the {@link IdentityKey} of the sender is untrusted.
         */
		public byte[] decrypt(PreKeySignalMessage ciphertext, DecryptionCallback callback)

		{
			lock (SESSION_LOCK)
			{
				SessionRecord sessionRecord = sessionStore.LoadSession(remoteAddress);
				May<uint> unsignedPreKeyId = sessionBuilder.process(sessionRecord, ciphertext);
				byte[] plaintext = decrypt(sessionRecord, ciphertext.getSignalMessage());

				callback.handlePlaintext(plaintext);

				sessionStore.StoreSession(remoteAddress, sessionRecord);

				if (unsignedPreKeyId.HasValue)
				{
					preKeyStore.RemovePreKey(unsignedPreKeyId.ForceGetValue());
				}

				return plaintext;
			}
		}

		/**
         * Decrypt a message.
         *
         * @param  ciphertext The {@link SignalMessage} to decrypt.
         *
         * @return The plaintext.
         * @throws InvalidMessageException if the input is not valid ciphertext.
         * @throws DuplicateMessageException if the input is a message that has already been received.
         * @throws LegacyMessageException if the input is a message formatted by a protocol version that
         *                                is no longer supported.
         * @throws NoSessionException if there is no established session for this contact.
         */
		public byte[] decrypt(SignalMessage ciphertext)

		{
			return decrypt(ciphertext, new NullDecryptionCallback());
		}

		/**
         * Decrypt a message.
         *
         * @param  ciphertext The {@link SignalMessage} to decrypt.
         * @param  callback   A callback that is triggered after decryption is complete,
         *                    but before the updated session state has been committed to the session
         *                    DB.  This allows some implementations to store the committed plaintext
         *                    to a DB first, in case they are concerned with a crash happening between
         *                    the time the session state is updated but before they're able to store
         *                    the plaintext to disk.
         *
         * @return The plaintext.
         * @throws InvalidMessageException if the input is not valid ciphertext.
         * @throws DuplicateMessageException if the input is a message that has already been received.
         * @throws LegacyMessageException if the input is a message formatted by a protocol version that
         *                                is no longer supported.
         * @throws NoSessionException if there is no established session for this contact.
         */
		public byte[] decrypt(SignalMessage ciphertext, DecryptionCallback callback)

		{
			lock (SESSION_LOCK)
			{

				if (!sessionStore.ContainsSession(remoteAddress))
				{
					throw new NoSessionException($"No session for: {remoteAddress}");
				}

				SessionRecord sessionRecord = sessionStore.LoadSession(remoteAddress);
				byte[] plaintext = decrypt(sessionRecord, ciphertext);

				callback.handlePlaintext(plaintext);

				sessionStore.StoreSession(remoteAddress, sessionRecord);

				return plaintext;
			}
		}

		private byte[] decrypt(SessionRecord sessionRecord, SignalMessage ciphertext)
		{
			lock (SESSION_LOCK)
			{
				IEnumerator<SessionState> previousStates = sessionRecord.getPreviousSessionStates().GetEnumerator();
				LinkedList<Exception> exceptions = new LinkedList<Exception>();

				try
				{
					SessionState sessionState = new SessionState(sessionRecord.getSessionState());
					byte[] plaintext = decrypt(sessionState, ciphertext);

					sessionRecord.setState(sessionState);
					return plaintext;
				}
				catch (InvalidMessageException e)
				{
					exceptions.AddLast(e);
				}

				while (previousStates.MoveNext())
				{
					try
					{
						SessionState promotedState = new SessionState(previousStates.Current);
						byte[] plaintext = decrypt(promotedState, ciphertext);

						sessionRecord.getPreviousSessionStates().Remove(previousStates.Current);
						sessionRecord.promoteState(promotedState);

						return plaintext;
					}
					catch (InvalidMessageException e)
					{
						exceptions.AddLast(e);
					}
				}

				throw new InvalidMessageException("No valid sessions.", exceptions);
			}
		}

		private byte[] decrypt(SessionState sessionState, SignalMessage ciphertextMessage)
		{
			if (!sessionState.hasSenderChain())
			{
				throw new InvalidMessageException("Uninitialized session!");
			}

			if (ciphertextMessage.getMessageVersion() != sessionState.getSessionVersion())
			{
				throw new InvalidMessageException($"Message version {ciphertextMessage.getMessageVersion()}, but session version {sessionState.getSessionVersion()}");
			}

			uint messageVersion = ciphertextMessage.getMessageVersion();
			ECPublicKey theirEphemeral = ciphertextMessage.getSenderRatchetKey();
			uint counter = ciphertextMessage.getCounter();
			ChainKey chainKey = getOrCreateChainKey(sessionState, theirEphemeral);
			MessageKeys messageKeys = getOrCreateMessageKeys(sessionState, theirEphemeral,
																	  chainKey, counter);

			ciphertextMessage.verifyMac(messageVersion,
											sessionState.getRemoteIdentityKey(),
											sessionState.getLocalIdentityKey(),
											messageKeys.getMacKey());

			byte[] plaintext = getPlaintext(messageVersion, messageKeys, ciphertextMessage.getBody());

			sessionState.clearUnacknowledgedPreKeyMessage();

			return plaintext;
		}

		public uint getRemoteRegistrationId()
		{
			lock (SESSION_LOCK)
			{
				SessionRecord record = sessionStore.LoadSession(remoteAddress);
				return record.getSessionState().getRemoteRegistrationId();
			}
		}

		public uint getSessionVersion()
		{
			lock (SESSION_LOCK)
			{
				if (!sessionStore.ContainsSession(remoteAddress))
				{
					throw new Exception($"No session for {remoteAddress}!");
				}

				SessionRecord record = sessionStore.LoadSession(remoteAddress);
				return record.getSessionState().getSessionVersion();
			}
		}

		private ChainKey getOrCreateChainKey(SessionState sessionState, ECPublicKey theirEphemeral)

		{
			try
			{
				if (sessionState.hasReceiverChain(theirEphemeral))
				{
					return sessionState.getReceiverChainKey(theirEphemeral);
				}
				else
				{
					RootKey rootKey = sessionState.getRootKey();
					ECKeyPair ourEphemeral = sessionState.getSenderRatchetKeyPair();
					Pair<RootKey, ChainKey> receiverChain = rootKey.createChain(theirEphemeral, ourEphemeral);
					ECKeyPair ourNewEphemeral = Curve.generateKeyPair();
					Pair<RootKey, ChainKey> senderChain = receiverChain.first().createChain(theirEphemeral, ourNewEphemeral);

					sessionState.setRootKey(senderChain.first());
					sessionState.addReceiverChain(theirEphemeral, receiverChain.second());
					sessionState.setPreviousCounter(Math.Max(sessionState.getSenderChainKey().getIndex() - 1, 0));
					sessionState.setSenderChain(ourNewEphemeral, senderChain.second());

					return receiverChain.second();
				}
			}
			catch (InvalidKeyException e)
			{
				throw new InvalidMessageException(e);
			}
		}

		private MessageKeys getOrCreateMessageKeys(SessionState sessionState,
												   ECPublicKey theirEphemeral,
												   ChainKey chainKey, uint counter)

		{
			if (chainKey.getIndex() > counter)
			{
				if (sessionState.hasMessageKeys(theirEphemeral, counter))
				{
					return sessionState.removeMessageKeys(theirEphemeral, counter);
				}
				else
				{
					throw new DuplicateMessageException($"Received message with old counter: {chainKey.getIndex()}  , {counter}");
				}
			}

			
			uint chainKeyIndex = chainKey.getIndex();
			if ((counter > chainKeyIndex) && (counter - chainKeyIndex > 2000))
			{
				throw new InvalidMessageException("Over 2000 messages into the future!");
			}

			while (chainKey.getIndex() < counter)
			{
				MessageKeys messageKeys = chainKey.getMessageKeys();
				sessionState.setMessageKeys(theirEphemeral, messageKeys);
				chainKey = chainKey.getNextChainKey();
			}

			sessionState.setReceiverChainKey(theirEphemeral, chainKey.getNextChainKey());
			return chainKey.getMessageKeys();
		}

		private byte[] getCiphertext(uint version, MessageKeys messageKeys, byte[] plaintext)
		{
			try
			{
				if (version >= 3)
				{
					return Encrypt.aesCbcPkcs5(plaintext, messageKeys.getCipherKey(), messageKeys.getIv());
				}
				else
				{
					return Encrypt.aesCtr(plaintext, messageKeys.getCipherKey(), messageKeys.getCounter());
				}
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		private byte[] getPlaintext(uint version, MessageKeys messageKeys, byte[] cipherText)
		{
			try
			{

				if (version >= 3)
				{
					return Decrypt.aesCbcPkcs5(cipherText, messageKeys.getCipherKey(), messageKeys.getIv());
				}
				else
				{
					return Decrypt.aesCtr(cipherText, messageKeys.getCipherKey(), messageKeys.getCounter());
				}
			}
			catch (Exception e)
			{
				throw new InvalidMessageException(e);
			}
		}

		private class NullDecryptionCallback : DecryptionCallback
		{

			public void handlePlaintext(byte[] plaintext) { }
		}
	}
}
