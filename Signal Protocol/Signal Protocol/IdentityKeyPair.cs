/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
using Google.ProtocolBuffers;
using libsignal.ecc;
using static libsignal.state.StorageProtos;

namespace libsignal
{
    /**
     * Holder for public and private identity key pair.
     *
     * @author
     */
    public class IdentityKeyPair
    {

        private readonly IdentityKey publicKey;
        private readonly ECPrivateKey privateKey;

        public IdentityKeyPair(IdentityKey publicKey, ECPrivateKey privateKey)
        {
            this.publicKey = publicKey;
            this.privateKey = privateKey;
        }

        public IdentityKeyPair(byte[] serialized)
        {
            try
            {
                IdentityKeyPairStructure structure = IdentityKeyPairStructure.ParseFrom(serialized);
                publicKey = new IdentityKey(structure.PublicKey.ToByteArray(), 0);
                privateKey = Curve.decodePrivatePoint(structure.PrivateKey.ToByteArray());
            }
            catch (InvalidProtocolBufferException e)
            {
                throw new InvalidKeyException(e);
            }
        }

        public IdentityKey getPublicKey()
        {
            return publicKey;
        }

        public ECPrivateKey getPrivateKey()
        {
            return privateKey;
        }

        public byte[] serialize()
        {
            return IdentityKeyPairStructure.CreateBuilder()
                                           .SetPublicKey(ByteString.CopyFrom(publicKey.serialize()))
                                           .SetPrivateKey(ByteString.CopyFrom(privateKey.serialize()))
                                           .Build().ToByteArray();
        }
    }
}
