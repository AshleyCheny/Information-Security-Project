/**
 * Copyright (C) 2013-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
using System.Linq;
using libsignal.util;

namespace libsignal.ecc
{
    public class DjbECPublicKey : ECPublicKey
    {
        private readonly byte[] publicKey;

        public DjbECPublicKey(byte[] publicKey)
        {
            this.publicKey = publicKey;
        }


        public byte[] serialize()
        {
            byte[] type = { Curve.DJB_TYPE };
            return ByteUtil.combine(type, publicKey);
        }


        public int getType()
        {
            return Curve.DJB_TYPE;
        }


        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (!(other is DjbECPublicKey)) return false;

            DjbECPublicKey that = (DjbECPublicKey)other;
            return Enumerable.SequenceEqual(publicKey, that.publicKey);
        }


        public override int GetHashCode()
        {
            return string.Join(",", publicKey).GetHashCode();
        }


        public int CompareTo(object another)
        {
            byte[] theirs = ((DjbECPublicKey)another).publicKey;
            string theirString = string.Join(",", theirs.Select(y => y.ToString()));
            string ourString = string.Join(",", publicKey.Select(y => y.ToString()));
            return ourString.CompareTo(theirString);
        }

        public byte[] getPublicKey()
        {
            return publicKey;
        }

    }
}
