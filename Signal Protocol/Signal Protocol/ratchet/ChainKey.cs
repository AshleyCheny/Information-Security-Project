/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
using libsignal.kdf;
using libsignal.util;
using System;
using System.Text;

namespace libsignal.ratchet
{
    public class ChainKey
    {

        private static readonly byte[] MESSAGE_KEY_SEED = { 0x01 };
        private static readonly byte[] CHAIN_KEY_SEED = { 0x02 };

        private readonly HKDF kdf;
        private readonly byte[] key;
        private readonly uint index;

        public ChainKey(HKDF kdf, byte[] key, uint index)
        {
            this.kdf = kdf;
            this.key = key;
            this.index = index;
        }

        public byte[] getKey()
        {
            return key;
        }

        public uint getIndex()
        {
            return index;
        }

        public ChainKey getNextChainKey()
        {
            byte[] nextKey = getBaseMaterial(CHAIN_KEY_SEED);
            return new ChainKey(kdf, nextKey, index + 1);
        }

        public MessageKeys getMessageKeys()
        {
            byte[] inputKeyMaterial = getBaseMaterial(MESSAGE_KEY_SEED);
            byte[] keyMaterialBytes = kdf.deriveSecrets(inputKeyMaterial, Encoding.UTF8.GetBytes("WhisperMessageKeys"), DerivedMessageSecrets.SIZE);
            DerivedMessageSecrets keyMaterial = new DerivedMessageSecrets(keyMaterialBytes);

            return new MessageKeys(keyMaterial.getCipherKey(), keyMaterial.getMacKey(), keyMaterial.getIv(), index);
        }

        private byte[] getBaseMaterial(byte[] seed)
        {
            try
            {
                return Sign.sha256sum(key, seed);
            }
            catch (InvalidKeyException e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
