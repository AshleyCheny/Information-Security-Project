/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
using libsignal.util;

namespace libsignal.kdf
{
    public class DerivedRootSecrets
    {
        public static readonly int SIZE = 64;

        private readonly byte[] rootKey;
        private readonly byte[] chainKey;

        public DerivedRootSecrets(byte[] okm)
        {
            byte[][] keys = ByteUtil.split(okm, 32, 32);
            rootKey = keys[0];
            chainKey = keys[1];
        }

        public byte[] getRootKey()
        {
            return rootKey;
        }

        public byte[] getChainKey()
        {
            return chainKey;
        }
    }
}
