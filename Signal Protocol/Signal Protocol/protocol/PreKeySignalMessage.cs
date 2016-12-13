/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
using Google.ProtocolBuffers;
using libsignal.ecc;
using libsignal.util;
using Strilanc.Value;
using System;

namespace libsignal.protocol
{
    public partial class PreKeySignalMessage : CiphertextMessage
    {

        private readonly uint version;
        private readonly uint registrationId;
        private readonly May<uint> preKeyId;
        private readonly uint signedPreKeyId;
        private readonly ECPublicKey baseKey;
        private readonly IdentityKey identityKey;
        private readonly SignalMessage message;
        private readonly byte[] serialized;

        public PreKeySignalMessage(byte[] serialized)
        {
            try
            {
                version = (uint)ByteUtil.highBitsToInt(serialized[0]);

                if (version > CURRENT_VERSION)
                {
                    throw new InvalidVersionException("Unknown version: " + version);
                }

      if (version < CURRENT_VERSION) {
        throw new LegacyMessageException("Legacy version: " + version);
      }
                WhisperProtos.PreKeySignalMessage preKeySignalMessage
                    = WhisperProtos.PreKeySignalMessage.ParseFrom(ByteString.CopyFrom(serialized, 1,
                                                                                       serialized.Length - 1));

                if (
                    !preKeySignalMessage.HasSignedPreKeyId ||
                    !preKeySignalMessage.HasBaseKey ||
                    !preKeySignalMessage.HasIdentityKey ||
                    !preKeySignalMessage.HasMessage)
                {
                    throw new InvalidMessageException("Incomplete message.");
                }

                this.serialized = serialized;
                registrationId = preKeySignalMessage.RegistrationId;
                preKeyId = preKeySignalMessage.HasPreKeyId ? new May<uint>(preKeySignalMessage.PreKeyId) : May<uint>.NoValue;
                signedPreKeyId = preKeySignalMessage.HasSignedPreKeyId ? preKeySignalMessage.SignedPreKeyId : uint.MaxValue;
                baseKey = Curve.decodePoint(preKeySignalMessage.BaseKey.ToByteArray(), 0);
                identityKey = new IdentityKey(Curve.decodePoint(preKeySignalMessage.IdentityKey.ToByteArray(), 0));
                message = new SignalMessage(preKeySignalMessage.Message.ToByteArray());
            }
            catch (Exception e)
            {
                throw new InvalidMessageException(e.Message);
            }
        }

        public PreKeySignalMessage(uint messageVersion, uint registrationId, May<uint> preKeyId,
                                    uint signedPreKeyId, ECPublicKey baseKey, IdentityKey identityKey,
                                    SignalMessage message)
        {
            version = messageVersion;
            this.registrationId = registrationId;
            this.preKeyId = preKeyId;
            this.signedPreKeyId = signedPreKeyId;
            this.baseKey = baseKey;
            this.identityKey = identityKey;
            this.message = message;

            WhisperProtos.PreKeySignalMessage.Builder builder =
                WhisperProtos.PreKeySignalMessage.CreateBuilder()
                                                  .SetSignedPreKeyId(signedPreKeyId)
                                                  .SetBaseKey(ByteString.CopyFrom(baseKey.serialize()))
                                                  .SetIdentityKey(ByteString.CopyFrom(identityKey.serialize()))
                                                  .SetMessage(ByteString.CopyFrom(message.serialize()))
                                                  .SetRegistrationId(registrationId);

            if (preKeyId.HasValue) // .isPresent()
            {
                builder.SetPreKeyId(preKeyId.ForceGetValue()); // get()
            }

            byte[] versionBytes = { ByteUtil.intsToByteHighAndLow((int)version, (int)CURRENT_VERSION) };
            byte[] messageBytes = builder.Build().ToByteArray();

            serialized = ByteUtil.combine(versionBytes, messageBytes);
        }

        public uint getMessageVersion()
        {
            return version;
        }

        public IdentityKey getIdentityKey()
        {
            return identityKey;
        }

        public uint getRegistrationId()
        {
            return registrationId;
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

        public SignalMessage getSignalMessage()
        {
            return message;
        }


        public override byte[] serialize()
        {
            return serialized;
        }


        public override uint getType()
        {
            return PREKEY_TYPE;
        }

    }
}
