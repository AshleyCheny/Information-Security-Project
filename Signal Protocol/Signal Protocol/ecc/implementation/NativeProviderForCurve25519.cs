using System;

namespace Signal_Protocol.ecc.implementation
{
    class NativeProviderForCurve25519 : IProviderForCurve25519
    {
        /// <summary>
        /// Calculate a ECDH 32-byte shared secret for the given public and private key
        /// </summary>
        /// <param name="ourPrivate"></param>
        /// <param name="theirPublic"></param>
        /// <returns></returns>
        public byte[] calculateAgreement(byte[] ourPrivate, byte[] theirPublic)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generate private key from the given random number
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        public byte[] generatePrivateKey(byte[] random)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generate public key from the given private key
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public byte[] generatePublicKey(byte[] privateKey)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Confirms if the current Curve25519 instance is native or not
        /// </summary>
        /// <returns></returns>
        public bool isNative()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}