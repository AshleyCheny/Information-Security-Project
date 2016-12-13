/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
using Google.ProtocolBuffers;
using libsignal.ecc;
using libsignal.util;
using System;
using System.IO;
using System.Linq;

namespace libsignal.protocol
{
    public partial class SignalMessage : CiphertextMessage
    {

        private static readonly int MAC_LENGTH = 8;

        private readonly uint messageVersion;
        private readonly ECPublicKey senderRatchetKey;
        private readonly uint counter;
        private readonly uint previousCounter;
        private readonly byte[] ciphertext;
        private readonly byte[] serialized;

        public SignalMessage(byte[] serialized)
        {
            try
            {
                byte[][] messageParts = ByteUtil.split(serialized, 1, serialized.Length - 1 - MAC_LENGTH, MAC_LENGTH);
                byte version = messageParts[0][0];
                byte[] message = messageParts[1];
                byte[] mac = messageParts[2];

                if (ByteUtil.highBitsToInt(version) <= UNSUPPORTED_VERSION)
                {
                    throw new LegacyMessageException("Legacy message: " + ByteUtil.highBitsToInt(version));
                }

                if (ByteUtil.highBitsToInt(version) > CURRENT_VERSION)
                {
                    throw new InvalidMessageException("Unknown version: " + ByteUtil.highBitsToInt(version));
                }

                WhisperProtos.SignalMessage SignalMessage = WhisperProtos.SignalMessage.ParseFrom(message);

                if (!SignalMessage.HasCiphertext ||
                    !SignalMessage.HasCounter ||
                    !SignalMessage.HasRatchetKey)
                {
                    throw new InvalidMessageException("Incomplete message.");
                }

                this.serialized = serialized;
                senderRatchetKey = Curve.decodePoint(SignalMessage.RatchetKey.ToByteArray(), 0);
                messageVersion = (uint)ByteUtil.highBitsToInt(version);
                counter = SignalMessage.Counter;
                previousCounter = SignalMessage.PreviousCounter;
                ciphertext = SignalMessage.Ciphertext.ToByteArray();
            }
            catch (/*InvalidProtocolBufferException | InvalidKeyException | Parse*/Exception e)
            {
                throw new InvalidMessageException(e);
            }
        }

        public SignalMessage(uint messageVersion, byte[] macKey, ECPublicKey senderRatchetKey,
                              uint counter, uint previousCounter, byte[] ciphertext,
                              IdentityKey senderIdentityKey,
                              IdentityKey receiverIdentityKey)
        {
            byte[] version = { ByteUtil.intsToByteHighAndLow((int)messageVersion, (int)CURRENT_VERSION) };
            byte[] message = WhisperProtos.SignalMessage.CreateBuilder()
                                           .SetRatchetKey(ByteString.CopyFrom(senderRatchetKey.serialize()))
                                           .SetCounter(counter)
                                           .SetPreviousCounter(previousCounter)
                                           .SetCiphertext(ByteString.CopyFrom(ciphertext))
                                           .Build().ToByteArray();

            byte[] mac = getMac(messageVersion, senderIdentityKey, receiverIdentityKey, macKey,
                                    ByteUtil.combine(version, message));

            serialized = ByteUtil.combine(version, message, mac);
            this.senderRatchetKey = senderRatchetKey;
            this.counter = counter;
            this.previousCounter = previousCounter;
            this.ciphertext = ciphertext;
            this.messageVersion = messageVersion;
        }

        public ECPublicKey getSenderRatchetKey()
        {
            return senderRatchetKey;
        }

        public uint getMessageVersion()
        {
            return messageVersion;
        }

        public uint getCounter()
        {
            return counter;
        }

        public byte[] getBody()
        {
            return ciphertext;
        }

        public void verifyMac(uint messageVersion, IdentityKey senderIdentityKey,
                        IdentityKey receiverIdentityKey, byte[] macKey)
        {
            byte[][] parts = ByteUtil.split(serialized, serialized.Length - MAC_LENGTH, MAC_LENGTH);
            byte[] ourMac = getMac(messageVersion, senderIdentityKey, receiverIdentityKey, macKey, parts[0]);
            byte[] theirMac = parts[1];

            if (!Enumerable.SequenceEqual(ourMac, theirMac))
            {
                throw new InvalidMessageException("Bad Mac!");
            }
        }

        private byte[] getMac(uint messageVersion,
                        IdentityKey senderIdentityKey,
                        IdentityKey receiverIdentityKey,
                        byte[] macKey, byte[] serialized)
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                if (messageVersion >= 3)
                {
                    byte[] sik = senderIdentityKey.getPublicKey().serialize();
                    stream.Write(sik, 0, sik.Length);
                    byte[] rik = receiverIdentityKey.getPublicKey().serialize();
                    stream.Write(rik, 0, rik.Length);
                }

                stream.Write(serialized, 0, serialized.Length);
                byte[] fullMac = Sign.sha256sum(macKey, stream.ToArray());
                return ByteUtil.trim(fullMac, MAC_LENGTH);
            }
            catch (/*NoSuchAlgorithmException | java.security.InvalidKey*/Exception  e)
            {
                throw new Exception(e.Message);
            }
        }

        public override byte[] serialize()
        {
            return serialized;
        }

        public override uint getType()
        {
            return WHISPER_TYPE;
        }

        public static bool isLegacy(byte[] message)
        {
            return message != null && message.Length >= 1 &&
                ByteUtil.highBitsToInt(message[0]) <= UNSUPPORTED_VERSION;
        }

    }
}
