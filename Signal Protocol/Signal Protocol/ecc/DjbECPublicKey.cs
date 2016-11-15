using Signal_Protocol.util;
using System;
using System.Linq;
using System.Numerics;

namespace Signal_Protocol.ecc
{
    internal class DjbECPublicKey : ECPublicKey
    {
        private byte[] publicKey;

        public DjbECPublicKey(byte[] publicKey)
        {
            this.publicKey = publicKey;
        }

        public int CompareTo(object another)
        {
            return new BigInteger(publicKey).CompareTo(new BigInteger(((DjbECPublicKey)another).publicKey));
        }

        public int getType()
        {
            return Curve.DJB_TYPE;
        }

        public bool equals(object other)
        {
            if (other == null) return false;
            if (!(other is DjbECPublicKey)) return false;

            DjbECPublicKey that = (DjbECPublicKey)other;
            return Enumerable.SequenceEqual(this.publicKey, that.publicKey);
        }

        public int hashCode()
        {
            return string.Join(",", publicKey).GetHashCode();
        }

        public byte[] serialize()
        {
            byte[] type = { Curve.DJB_TYPE };
            return ByteUtil.combine(type, publicKey);
        }

        public byte[] getPublicKey()
        {
            return publicKey;
        }
    }
}