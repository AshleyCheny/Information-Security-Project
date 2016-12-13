/**
 * Copyright (C) 2013-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
namespace libsignal.ecc
{
    public class DjbECPrivateKey : ECPrivateKey
    {

        private readonly byte[] privateKey;

        public DjbECPrivateKey(byte[] privateKey)
        {
            this.privateKey = privateKey;
        }


        public byte[] serialize()
        {
            return privateKey;
        }


        public int getType()
        {
            return Curve.DJB_TYPE;
        }

        public byte[] getPrivateKey()
        {
            return privateKey;
        }
    }
}
