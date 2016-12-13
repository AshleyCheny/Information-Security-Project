/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
 using libsignal.ecc;
using System;

namespace libsignal
{
    /**
     * A class for representing an identity key.
     * 
     * @author Moxie Marlinspike
     */

    public class IdentityKey
    {

        private ECPublicKey publicKey;

        public IdentityKey(ECPublicKey publicKey)
        {
            this.publicKey = publicKey;
        }

        public IdentityKey(byte[] bytes, int offset)
        {
            publicKey = Curve.decodePoint(bytes, offset);
        }

        public ECPublicKey getPublicKey()
        {
            return publicKey;
        }

        public byte[] serialize()
        {
            return publicKey.serialize();
        }

        public string getFingerprint()
        {
            return publicKey.serialize().ToString();
        }

        public override bool Equals(Object other)
        {
            if (other == null) return false;
            if (!(other is IdentityKey)) return false;

            return publicKey.Equals(((IdentityKey)other).getPublicKey());
        }


        public override int GetHashCode()
        {
            return publicKey.GetHashCode();
        }
    }
}
