/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
using Google.ProtocolBuffers;
using libsignal.ecc;
using libsignal.util;
using System;

namespace libsignal.protocol
{
    public partial class SenderKeyMessage : CiphertextMessage
    {

        private static readonly int SIGNATURE_LENGTH = 64;

        private readonly uint messageVersion;
        private readonly uint keyId;
        private readonly uint iteration;
        private readonly byte[] ciphertext;
        private readonly byte[] serialized;

        public SenderKeyMessage(byte[] serialized)
        {
            try
            {
                byte[][] messageParts = ByteUtil.split(serialized, 1, serialized.Length - 1 - SIGNATURE_LENGTH, SIGNATURE_LENGTH);
                byte version = messageParts[0][0];
                byte[] message = messageParts[1];
                byte[] signature = messageParts[2];

                if (ByteUtil.highBitsToInt(version) < 3)
                {
                    throw new LegacyMessageException("Legacy message: " + ByteUtil.highBitsToInt(version));
                }

                if (ByteUtil.highBitsToInt(version) > CURRENT_VERSION)
                {
                    throw new InvalidMessageException("Unknown version: " + ByteUtil.highBitsToInt(version));
                }

                WhisperProtos.SenderKeyMessage senderKeyMessage = WhisperProtos.SenderKeyMessage.ParseFrom(message);

                if (!senderKeyMessage.HasId ||
                    !senderKeyMessage.HasIteration ||
                    !senderKeyMessage.HasCiphertext)
                {
                    throw new InvalidMessageException("Incomplete message.");
                }

                this.serialized = serialized;
                messageVersion = (uint)ByteUtil.highBitsToInt(version);
                keyId = senderKeyMessage.Id;
                iteration = senderKeyMessage.Iteration;
                ciphertext = senderKeyMessage.Ciphertext.ToByteArray();
            }
            catch (/*InvalidProtocolBufferException | Parse*/Exception e)
            {
                throw new InvalidMessageException(e);
            }
        }

        public SenderKeyMessage(uint keyId, uint iteration, byte[] ciphertext, ECPrivateKey signatureKey)
        {
            byte[] version = { ByteUtil.intsToByteHighAndLow((int)CURRENT_VERSION, (int)CURRENT_VERSION) };
            byte[] message = WhisperProtos.SenderKeyMessage.CreateBuilder()
                                                           .SetId(keyId)
                                                           .SetIteration(iteration)
                                                           .SetCiphertext(ByteString.CopyFrom(ciphertext))
                                                           .Build().ToByteArray();

            byte[] signature = getSignature(signatureKey, ByteUtil.combine(version, message));

            serialized = ByteUtil.combine(version, message, signature);
            messageVersion = CURRENT_VERSION;
            this.keyId = keyId;
            this.iteration = iteration;
            this.ciphertext = ciphertext;
        }

        public uint getKeyId()
        {
            return keyId;
        }

        public uint getIteration()
        {
            return iteration;
        }

        public byte[] getCipherText()
        {
            return ciphertext;
        }

        public void verifySignature(ECPublicKey signatureKey)
        {
            try
            {
                byte[][] parts = ByteUtil.split(serialized, serialized.Length - SIGNATURE_LENGTH, SIGNATURE_LENGTH);

                if (!Curve.verifySignature(signatureKey, parts[0], parts[1]))
                {
                    throw new InvalidMessageException("Invalid signature!");
                }

            }
            catch (InvalidKeyException e)
            {
                throw new InvalidMessageException(e);
            }
        }

        private byte[] getSignature(ECPrivateKey signatureKey, byte[] serialized)
        {
            try
            {
                return Curve.calculateSignature(signatureKey, serialized);
            }
            catch (InvalidKeyException e)
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
            return SENDERKEY_TYPE;
        }
    }
}
