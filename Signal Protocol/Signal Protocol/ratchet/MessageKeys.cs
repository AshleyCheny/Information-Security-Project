/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
namespace libsignal.ratchet
{
    public class MessageKeys
    {

        private readonly byte[] cipherKey;
        private readonly byte[] macKey;
        private readonly byte[] iv;
        private readonly uint counter;

        public MessageKeys(byte[] cipherKey, byte[] macKey, byte[] iv, uint counter)
        {
            this.cipherKey = cipherKey;
            this.macKey = macKey;
            this.iv = iv;
            this.counter = counter;
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

        public uint getCounter()
        {
            return counter;
        }
    }
}
