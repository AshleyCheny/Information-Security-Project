using org.whispersystems.curve25519;
namespace Signal_Protocol.encryptioncurve.implementation
{
    class ManagedProviderForCurve25519 : IProviderCurve25519
    {
        private Curve25519 curve;

        public ManagedProviderForCurve25519(string type)
        {
            curve = Curve25519.getInstance(type);
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