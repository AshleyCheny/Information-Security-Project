/**
 * Copyright (C) 2013-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
using libsignal.ecc.impl;
using static PCLCrypto.WinRTCrypto;

namespace libsignal.ecc
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="type">Such as Curve25519.CSHARP or Curve25519.BEST</param>
    public enum Curve25519ProviderType
	{
		BEST = 0x05,
		NATIVE
	}

	class Curve25519
	{
		private static Curve25519 instance;
		private ICurve25519Provider provider;

		private Curve25519() { }
        
		public static Curve25519 getInstance(Curve25519ProviderType type)
		{
			if (instance == null)
            {
                instance = new Curve25519();
                switch (type)
                {
                    case Curve25519ProviderType.NATIVE:
                        {
                            instance.provider = new Curve25519NativeProvider();
                            break;
                        }
                    case Curve25519ProviderType.BEST:
                        {
                            instance.provider = new Curve25519ManagedProvider(
                                org.whispersystems.curve25519.Curve25519.BEST);
                            break;
                        }
                }
			}
			return instance;
		}
        
		public bool isNative()
		{
			return provider.isNative();
		}
        
		public Curve25519KeyPair generateKeyPair()
		{
            byte[] random = CryptographicBuffer.GenerateRandom(32);
			byte[] privateKey = provider.generatePrivateKey(random);
			byte[] publicKey = provider.generatePublicKey(privateKey);

			return new Curve25519KeyPair(publicKey, privateKey);
		}
        
		public byte[] calculateAgreement(byte[] publicKey, byte[] privateKey)
		{
			return provider.calculateAgreement(privateKey, publicKey);
		}
        
		public byte[] calculateSignature(byte[] privateKey, byte[] message)
		{

            byte[] random = CryptographicBuffer.GenerateRandom(64);
			return provider.calculateSignature(random, privateKey, message);
		}
        
		public bool verifySignature(byte[] publicKey, byte[] message, byte[] signature)
		{
			return provider.verifySignature(publicKey, message, signature);
		}
	}
    
	public class Curve25519KeyPair
	{

		private readonly byte[] publicKey;
		private readonly byte[] privateKey;
        
		public Curve25519KeyPair(byte[] publicKey, byte[] privateKey)
		{
			this.publicKey = publicKey;
			this.privateKey = privateKey;
		}
        
		public byte[] getPublicKey()
		{
			return publicKey;
		}
        
		public byte[] getPrivateKey()
		{
			return privateKey;
		}
	}
}
