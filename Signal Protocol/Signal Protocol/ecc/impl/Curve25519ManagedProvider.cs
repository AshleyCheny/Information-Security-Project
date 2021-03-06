﻿namespace libsignal.ecc.impl
{
    class Curve25519ManagedProvider : ICurve25519Provider
    {
        private org.whispersystems.curve25519.Curve25519 curve;

        public Curve25519ManagedProvider(string type)
        {
            curve = org.whispersystems.curve25519.Curve25519.getInstance(type);
        }

        public byte[] calculateAgreement(byte[] ourPrivate, byte[] theirPublic)
        {
            return curve.calculateAgreement(ourPrivate, theirPublic);
        }

        public byte[] calculateSignature(byte[] random, byte[] privateKey, byte[] message)
        {
            return curve.calculateSignature(random, privateKey, message);
        }

        public byte[] generatePrivateKey(byte[] random)
        {
            return curve.generatePrivateKey(random);
        }

        public byte[] generatePublicKey(byte[] privateKey)
        {
            return curve.generatePublicKey(privateKey);
        }

        public bool isNative()
        {
            return curve.isNative();
        }

        public bool verifySignature(byte[] publicKey, byte[] message, byte[] signature)
        {
            return curve.verifySignature(publicKey, message, signature);
        }
    }
}
