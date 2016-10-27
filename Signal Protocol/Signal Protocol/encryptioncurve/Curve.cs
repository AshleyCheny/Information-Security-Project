using org.whispersystems.curve25519;

namespace Signal_Protocol.encryptioncurve
{
    /// <summary>
    /// Class Curve25519 to implement functionalities related to Key generation
    /// </summary>
    public class Curve
    {
        public const int DJB_TYPE = 0x05;

        /// <summary>
        /// Class to confirm if the current Curve25519 instance is native or not
        /// </summary>
        /// <returns></returns>
        public static bool isNative()
        {
            return Curve25519.getInstance(Curve25519ProviderType.BEST).isNative();
        }

        /// <summary>
        /// Functionality yo generate ket pair using the best instance of Curve25519
        /// </summary>
        /// <returns></returns>
        public static ECKeyPair generateKeyPair()
        {
            Curve25519KeyPair keyPair = Curve25519.getInstance(Curve25519ProviderType.BEST).generateKeyPair();

            return new ECKeyPair(new DjbECPublicKey(keyPair.getPublicKey()),
                                 new DjbECPrivateKey(keyPair.getPrivateKey()));
        }

        /// <summary>
        /// Decodes the bytes to generate the encryption public key
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Decodes the bytes to generate the encryption private key
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static ECPrivateKey decodePrivatePoint(byte[] bytes)
        {
            return new DjbECPrivateKey(bytes);
        }

        /// <summary>
        /// Calculate a ECDH 32-byte shared secret for the given public and private key
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Verifies the signature of a given message using the Curve25519 public key the message belongs to
        /// </summary>
        /// <param name="signingKey"></param>
        /// <param name="message"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Calculates a 64-byte Curve25519 signature with the private key and a random number
        /// </summary>
        /// <param name="signingKey"></param>
        /// <param name="message"></param>
        /// <returns></returns>
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
