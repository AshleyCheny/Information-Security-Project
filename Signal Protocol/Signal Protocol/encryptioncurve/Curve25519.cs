﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal_Protocol.encryptioncurve
{
    public enum Curve25519ProviderType
    {
        // Use the best available implementation in case the native implementation is not available
        BEST = 0x05,
        // Use Native impplementation compulsorily
        NATIVE
    }
    class Curve25519
    {
        private static Curve25519 instance;
        private ICurve25519Provider provider;

        private Curve25519() { }

        /// <summary>
        /// Gets the current Curve25519 instance implementation based on the type of provider
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Curve25519 getInstance(Curve25519ProviderType type)
        {
            if (instance == null)
            {
                instance = new Curve25519();
                switch (type)
                {
                    case Curve25519ProviderType.NATIVE:
                        {
                            instance.provider = (ICurve25519Provider)new Curve25519NativeProvider();
                            break;
                        }
                    case Curve25519ProviderType.BEST:
                        {
                            instance.provider = (ICurve25519Provider)new Curve25519ManagedProvider(
                                org.whispersystems.curve25519.Curve25519.BEST);
                            break;
                        }
                }
            }
            return instance;
        }

        /// <summary>
        /// Returns true if the provider implementing Curve25519 is native
        /// </summary>
        /// <returns></returns>
        public bool isNative()
        {
            return provider.isNative();
        }

        /// <summary>
        /// Function to generate random Curve25519 keypair
        /// </summary>
        /// <returns></returns>
        public Curve25519KeyPair generateKeyPair()
        {
            byte[] random = CryptographicBuffer.GenerateRandom(32);
            byte[] privateKey = provider.generatePrivateKey(random);
            byte[] publicKey = provider.generatePublicKey(privateKey);

            return new Curve25519KeyPair(publicKey, privateKey);
        }

        /// <summary>
        /// Calculate a ECDH 32-byte shared secret for the given public and private key
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public byte[] calculateAgreement(byte[] publicKey, byte[] privateKey)
        {
            return provider.calculateAgreement(privateKey, publicKey);
        }

        /// <summary>
        /// Calculates a 64-byte Curve25519 signature with the private key and a random number
        /// The signature is signed on the message provided
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public byte[] calculateSignature(byte[] privateKey, byte[] message)
        {

            byte[] random = CryptographicBuffer.GenerateRandom(64);
            return provider.calculateSignature(random, privateKey, message);
        }

        /// <summary>
        /// Verifies the signature of a given message using the Curve25519 public key the message bellongs to
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="message"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        public bool verifySignature(byte[] publicKey, byte[] message, byte[] signature)
        {
            return provider.verifySignature(publicKey, message, signature);
        }
    }

    /// <summary>
    /// Class to create a Curve25519 key pair from the given public and private key
    /// </summary>
    public class KeyPairForCurve25519
    {
        private readonly byte[] publicKey;
        private readonly byte[] privateKey;

        /// <summary>
        /// Constructor of KeyPairForCurve25519 class
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="privateKey"></param>
        public KeyPairForCurve25519(byte[] publicKey, byte[] privateKey)
        {
            this.publicKey = publicKey;
            this.privateKey = privateKey;
        }

        /// <summary>
        /// Curve25519 public key property
        /// </summary>
        /// <returns></returns>
        public byte[] getPublicKey()
        {
            return publicKey;
        }

        /// <summary>
        /// Curve25519 private key property
        /// </summary>
        /// <returns></returns>
        public byte[] getPrivateKey()
        {
            return privateKey;
        }
    }
}
