namespace Signal_Protocol.encryptioncurve.implementation
{
    /// <summary>
    /// Interface to restrict the exposure of elliptical curve 25519 implementation
    /// </summary>
    interface IProviderForCurve25519
    {
        byte[] calculateAgreement(byte[] ourPrivate, byte[] theirPublic);
        byte[] calculateSignature(byte[] random, byte[] privateKey, byte[] message);
        byte[] generatePrivateKey(byte[] random);
        byte[] generatePublicKey(byte[] privateKey);
        bool isNative();
        bool verifySignature(byte[] publicKey, byte[] message, byte[] signature);
    }
}