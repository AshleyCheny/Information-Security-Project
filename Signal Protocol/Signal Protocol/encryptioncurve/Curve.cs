﻿using org.whispersystems.curve25519;

namespace Signal_Protocol.encryptioncurve
{
    public class Curve
    {
        public const int DJB_TYPE = 0x05;

        public static bool isNative()
        {
            return Curve25519.getInstance(Curve25519ProviderType.BEST).isNative();
        }

        public static ECKeyPair generateKeyPair()
        {
            Curve25519KeyPair keyPair = Curve25519.getInstance(Curve25519ProviderType.BEST).generateKeyPair();

            return new ECKeyPair(new DjbECPublicKey(keyPair.getPublicKey()),
                                 new DjbECPrivateKey(keyPair.getPrivateKey()));
        }

        public static ECPublicKey decodePoint(byte[] bytes, int offset)
        {
            int type = bytes[offset] & 0xFF;

            switch (type)
            {
                case Curve.DJB_TYPE:
                    byte[] keyBytes = new byte[32];
                    System.Buffer.BlockCopy(bytes, offset + 1, keyBytes, 0, keyBytes.Length);
                    return new DjbECPublicKey(keyBytes);
                default:
                    throw new InvalidKeyException("Bad key type: " + type);
            }
        }

        public static ECPrivateKey decodePrivatePoint(byte[] bytes)
        {
            return new DjbECPrivateKey(bytes);
        }

        public static byte[] calculateAgreement(ECPublicKey publicKey, ECPrivateKey privateKey)
        {
            if (publicKey.getType() != privateKey.getType())
            {
                throw new InvalidKeyException("Public and private keys must be of the same type!");
            }

            if (publicKey.getType() == DJB_TYPE)
            {
                return Curve25519.getInstance(Curve25519ProviderType.BEST)
                                 .calculateAgreement(((DjbECPublicKey)publicKey).getPublicKey(),
                                                     ((DjbECPrivateKey)privateKey).getPrivateKey());
            }
            else
            {
                throw new InvalidKeyException("Unknown type: " + publicKey.getType());
            }
        }

        public static bool verifySignature(ECPublicKey signingKey, byte[] message, byte[] signature)
        {
            if (signingKey.getType() == DJB_TYPE)
            {
                return Curve25519.getInstance(Curve25519ProviderType.BEST)
                                 .verifySignature(((DjbECPublicKey)signingKey).getPublicKey(), message, signature);
            }
            else
            {
                throw new InvalidKeyException("Unknown type: " + signingKey.getType());
            }
        }

        public static byte[] calculateSignature(ECPrivateKey signingKey, byte[] message)
        {
            if (signingKey.getType() == DJB_TYPE)
            {
                return Curve25519.getInstance(Curve25519ProviderType.BEST)
                                 .calculateSignature(((DjbECPrivateKey)signingKey).getPrivateKey(), message);
            }
            else
            {
                throw new InvalidKeyException("Unknown type: " + signingKey.getType());
            }
        }
    }
}
