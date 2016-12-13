
using Google.ProtocolBuffers;
using libsignal.ecc;
using libsignal.util;

namespace libsignal.protocol
{
    public class KeyExchangeMessage
    {

        public static readonly uint INITIATE_FLAG = 0x01;
        public static readonly uint RESPONSE_FLAG = 0X02;
        public static readonly uint SIMULTAENOUS_INITIATE_FLAG = 0x04;

        private readonly uint version;
        private readonly uint supportedVersion;
        private readonly uint sequence;
        private readonly uint flags;

        private readonly ECPublicKey baseKey;
        private readonly byte[] baseKeySignature;
        private readonly ECPublicKey ratchetKey;
        private readonly IdentityKey identityKey;
        private readonly byte[] serialized;

        public KeyExchangeMessage(uint messageVersion, uint sequence, uint flags,
                                  ECPublicKey baseKey, byte[] baseKeySignature,
                                  ECPublicKey ratchetKey,
                                  IdentityKey identityKey)
        {
            supportedVersion = CiphertextMessage.CURRENT_VERSION;
            this.version = messageVersion;
            this.sequence = sequence;
            this.flags = flags;
            this.baseKey = baseKey;
            this.baseKeySignature = baseKeySignature;
            this.ratchetKey = ratchetKey;
            this.identityKey = identityKey;

            byte[] version = { ByteUtil.intsToByteHighAndLow((int)this.version, (int)supportedVersion) };
            WhisperProtos.KeyExchangeMessage.Builder builder = WhisperProtos.KeyExchangeMessage
                                           .CreateBuilder()
                                           .SetId((sequence << 5) | flags)
                                           .SetBaseKey(ByteString.CopyFrom(baseKey.serialize()))
                                           .SetRatchetKey(ByteString.CopyFrom(ratchetKey.serialize()))
                                           .SetIdentityKey(ByteString.CopyFrom(identityKey.serialize()));

            if (messageVersion >= 3)
            {
                builder.SetBaseKeySignature(ByteString.CopyFrom(baseKeySignature));
            }

            serialized = ByteUtil.combine(version, builder.Build().ToByteArray());
        }

        public KeyExchangeMessage(byte[] serialized)
        {
            try
            {
                byte[][] parts = ByteUtil.split(serialized, 1, serialized.Length - 1);
                version = (uint)ByteUtil.highBitsToInt(parts[0][0]);
                supportedVersion = (uint)ByteUtil.lowBitsToInt(parts[0][0]);

                if (version <= CiphertextMessage.UNSUPPORTED_VERSION)
                {
                    throw new LegacyMessageException("Unsupported legacy version: " + version);
                }

                if (version > CiphertextMessage.CURRENT_VERSION)
                {
                    throw new InvalidVersionException("Unknown version: " + version);
                }

                WhisperProtos.KeyExchangeMessage message = WhisperProtos.KeyExchangeMessage.ParseFrom(parts[1]);

                if (!message.HasId || !message.HasBaseKey ||
                    !message.HasRatchetKey || !message.HasIdentityKey ||
                    (version >= 3 && !message.HasBaseKeySignature))
                {
                    throw new InvalidMessageException("Some required fields missing!");
                }

                sequence = message.Id >> 5;
                flags = message.Id & 0x1f;
                this.serialized = serialized;
                baseKey = Curve.decodePoint(message.BaseKey.ToByteArray(), 0);
                baseKeySignature = message.BaseKeySignature.ToByteArray();
                ratchetKey = Curve.decodePoint(message.RatchetKey.ToByteArray(), 0);
                identityKey = new IdentityKey(message.IdentityKey.ToByteArray(), 0);
            }
            catch (InvalidKeyException e)
            {
                throw new InvalidMessageException(e);
            }
        }

        public uint getVersion()
        {
            return version;
        }

        public ECPublicKey getBaseKey()
        {
            return baseKey;
        }

        public byte[] getBaseKeySignature()
        {
            return baseKeySignature;
        }

        public ECPublicKey getRatchetKey()
        {
            return ratchetKey;
        }

        public IdentityKey getIdentityKey()
        {
            return identityKey;
        }

        public bool hasIdentityKey()
        {
            return true;
        }

        public uint getMaxVersion()
        {
            return supportedVersion;
        }

        public bool isResponse()
        {
            return ((flags & RESPONSE_FLAG) != 0);
        }

        public bool isInitiate()
        {
            return (flags & INITIATE_FLAG) != 0;
        }

        public bool isResponseForSimultaneousInitiate()
        {
            return (flags & SIMULTAENOUS_INITIATE_FLAG) != 0;
        }

        public uint getFlags()
        {
            return flags;
        }

        public uint getSequence()
        {
            return sequence;
        }

        public byte[] serialize()
        {
            return serialized;
        }
    }
}
