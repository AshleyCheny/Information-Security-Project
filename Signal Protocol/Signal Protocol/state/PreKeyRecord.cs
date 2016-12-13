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
    public class PreKeyRecord
    {

        private PreKeyRecordStructure structure;

        public PreKeyRecord(uint id, ECKeyPair keyPair)
        {
            structure = PreKeyRecordStructure.CreateBuilder()
                                                  .SetId(id)
                                                  .SetPublicKey(ByteString.CopyFrom(keyPair.getPublicKey()
                                                                                           .serialize()))
                                                  .SetPrivateKey(ByteString.CopyFrom(keyPair.getPrivateKey()
                                                                                            .serialize()))
                                                  .Build();
        }

        public PreKeyRecord(byte[] serialized)
        {
            structure = PreKeyRecordStructure.ParseFrom(serialized);
        }



        public uint getId()
        {
            return structure.Id;
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

        public byte[] serialize()
        {
            return structure.ToByteArray();
        }
    }
}
