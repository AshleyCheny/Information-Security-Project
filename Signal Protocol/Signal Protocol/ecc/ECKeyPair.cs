/**
 * Copyright (C) 2013-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
namespace libsignal.ecc
{
    public class ECKeyPair
    {

        private readonly ECPublicKey publicKey;
        private readonly ECPrivateKey privateKey;

        public ECKeyPair(ECPublicKey publicKey, ECPrivateKey privateKey)
        {
            this.publicKey = publicKey;
            this.privateKey = privateKey;
        }

        public ECPublicKey getPublicKey()
        {
            return publicKey;
        }

        public ECPrivateKey getPrivateKey()
        {
            return privateKey;
        }
    }
}
