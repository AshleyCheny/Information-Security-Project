namespace Signal_Protocol.ecc.implementation
{
    /// <summary>
    /// Class to handle Curve25519 for managed providers
    /// </summary>
    class ManagedProviderForCurve25519 : IProviderForCurve25519
    {
        private org.whispersystems.curve25519.Curve25519 curve;

        /// <summary>
        /// Constructor for the ManagedProviderForCurve25519 class
        /// </summary>
        /// <param name="type"></param>
        public ManagedProviderForCurve25519(string type)
        {
            curve = org.whispersystems.curve25519.Curve25519.getInstance(type);
        }

        /// <summary>
        /// Calculate a ECDH 32-byte shared secret for the given public and private key
        /// </summary>
        /// <param name="ourPrivate"></param>
        /// <param name="theirPublic"></param>
        /// <returns></returns>
        public byte[] calculateAgreement(byte[] ourPrivate, byte[] theirPublic)
        {
            return curve.calculateAgreement(ourPrivate, theirPublic);
        }

        /// <summary>
        /// Calculates a 64-byte Curve25519 signature with the private key and a random number
        /// </summary>
        /// <param name="random"></param>
        /// <param name="privateKey"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public byte[] calculateSignature(byte[] random, byte[] privateKey, byte[] message)
        {
            return curve.calculateSignature(random, privateKey, message);
        }

        /// <summary>
        /// Generate private key from the given random number
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        public byte[] generatePrivateKey(byte[] random)
        {
            return curve.generatePrivateKey(random);
        }

        /// <summary>
        /// Generate public key from the given private key
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public byte[] generatePublicKey(byte[] privateKey)
        {
            return curve.generatePublicKey(privateKey);
        }

        /// <summary>
        /// Confirms if the current Curve25519 instance is native or not
        /// </summary>
        /// <returns></returns>
        public bool isNative()
        {
            return curve.isNative();
        }

        /// <summary>
        /// Verifies the signature of a given message using the Curve25519 public key the message belongs to
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="message"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        public bool verifySignature(byte[] publicKey, byte[] message, byte[] signature)
        {
            return curve.verifySignature(publicKey, message, signature);
        }
    }
}