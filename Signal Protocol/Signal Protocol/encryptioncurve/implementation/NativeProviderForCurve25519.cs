﻿using System;

namespace Signal_Protocol.encryptioncurve.implementation
{
    class NativeProviderForCurve25519 : IProviderForCurve25519
    {

        public byte[] calculateAgreement(byte[] ourPrivate, byte[] theirPublic)
        {
            throw new NotImplementedException();
        }

        public byte[] calculateSignature(byte[] random, byte[] privateKey, byte[] message)
        {
            throw new NotImplementedException();
        }

        public byte[] generatePrivateKey(byte[] random)
        {
            throw new NotImplementedException();
        }

        public byte[] generatePublicKey(byte[] privateKey)
        {
            throw new NotImplementedException();
        }

        public bool isNative()
        {
            throw new NotImplementedException();
        }

        public bool verifySignature(byte[] publicKey, byte[] message, byte[] signature)
        {
            throw new NotImplementedException();
        }
    }
}
