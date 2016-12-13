/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
using libsignal.util;

namespace libsignal.kdf
{
    public class DerivedMessageSecrets
    {

        public static readonly int SIZE = 80;
        private static readonly int CIPHER_KEY_LENGTH = 32;
        private static readonly int MAC_KEY_LENGTH = 32;
        private static readonly int IV_LENGTH = 16;

        private readonly byte[] cipherKey;
        private readonly byte[] macKey;
        private readonly byte[] iv;

        public DerivedMessageSecrets(byte[] okm)
        {
            byte[][] keys = ByteUtil.split(okm, CIPHER_KEY_LENGTH, MAC_KEY_LENGTH, IV_LENGTH);

            cipherKey = keys[0];
            macKey = keys[1];
            iv = keys[2];
        }

        public byte[] getCipherKey()
        {
            return cipherKey;
        }

        public byte[] getMacKey()
        {
            return macKey;
        }

        public byte[] getIv()
        {
            return iv;
        }
    }
}
