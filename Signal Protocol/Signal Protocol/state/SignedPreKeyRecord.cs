/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
 using Google.ProtocolBuffers;
using libsignal.ecc;
using System;
using static libsignal.state.StorageProtos;

namespace libsignal.state
{
    public class SignedPreKeyRecord
    {

        private SignedPreKeyRecordStructure structure;

        public SignedPreKeyRecord(uint id, ulong timestamp, ECKeyPair keyPair, byte[] signature)
        {
            structure = SignedPreKeyRecordStructure.CreateBuilder()
                                                        .SetId(id)
                                                        .SetPublicKey(ByteString.CopyFrom(keyPair.getPublicKey()
                                                                                                 .serialize()))
                                                        .SetPrivateKey(ByteString.CopyFrom(keyPair.getPrivateKey()
                                                                                                  .serialize()))
                                                        .SetSignature(ByteString.CopyFrom(signature))
                                                        .SetTimestamp(timestamp)
                                                        .Build();
        }

        public SignedPreKeyRecord(byte[] serialized)
        {
            structure = SignedPreKeyRecordStructure.ParseFrom(serialized);
        }

        public uint getId()
        {
            return structure.Id;
        }

        public ulong getTimestamp()
        {
            return structure.Timestamp;
        }

        public ECKeyPair getKeyPair()
        {
            try
            {
                ECPublicKey publicKey = Curve.decodePoint(structure.PublicKey.ToByteArray(), 0);
                ECPrivateKey privateKey = Curve.decodePrivatePoint(structure.PrivateKey.ToByteArray());

                return new ECKeyPair(publicKey, privateKey);
            }
            catch (InvalidKeyException e)
            {
                throw new Exception(e.Message);
            }
        }

        public byte[] getSignature()
        {
            return structure.Signature.ToByteArray();
        }

        public byte[] serialize()
        {
            return structure.ToByteArray();
        }
    }
}
