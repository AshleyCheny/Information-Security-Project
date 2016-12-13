 namespace libsignal.ecc.impl
{
	public interface ICurve25519Provider
	{
		byte[] calculateAgreement(byte[] ourPrivate, byte[] theirPublic);
		byte[] calculateSignature(byte[] random, byte[] privateKey, byte[] message);
		byte[] generatePrivateKey(byte[] random);
		byte[] generatePublicKey(byte[] privateKey);
		bool isNative();
		bool verifySignature(byte[] publicKey, byte[] message, byte[] signature);
	}
}