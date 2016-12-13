using System;
using System.Collections.Generic;
using static PCLCrypto.WinRTCrypto;
using PCLCrypto;

namespace libsignal.util
{
    public class Sign
    {
        public static byte[] sha256sum(byte[] key, byte[] message)
        {
            IMacAlgorithmProvider provider = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithm.HmacSha256);
            ICryptographicKey hmacKey = provider.CreateKey(key);
            byte [] hmac = CryptographicEngine.Sign(hmacKey, message);
            return hmac;
        }
    }

    public class Sha256
    {
        public static byte[] Sign(byte[] key, byte[] message)
        {
            IMacAlgorithmProvider provider = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithm.HmacSha256);
            ICryptographicKey hmacKey = provider.CreateKey(key);
            byte[] hmac = CryptographicEngine.Sign(hmacKey, message);
            return hmac;
        }

        public static bool Verify(byte[] key, byte[] message, byte[] signature)
        {
            IMacAlgorithmProvider provider = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithm.HmacSha256);

            ICryptographicKey hmacKey = provider.CreateKey(key);
            return CryptographicEngine.VerifySignature(hmacKey, message, signature);
        }
    }

    /// <summary>
    /// Encryption helpers
    /// </summary>
    public class Encrypt
    {
        /// <summary>
        /// Computes PKCS5 for the message
        /// </summary>
        /// <param name="message">plaintext</param>
        /// <returns>PKCS5 of the message</returns>
        public static byte[] aesCbcPkcs5(byte[] message, byte[] key, byte[] iv)
        {
            ISymmetricKeyAlgorithmProvider objAlg = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7); // PKCS5
            ICryptographicKey ckey = objAlg.CreateSymmetricKey(key);
            byte [] result = CryptographicEngine.Encrypt(ckey, message, iv);
            return result;
        }

        public static byte[] aesCtr(byte[] message, byte[] key, uint counter)
        {
            ISymmetricKeyAlgorithmProvider objAlg = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7); // CRT
            ICryptographicKey ckey = objAlg.CreateSymmetricKey(key);

            byte[] ivBytes = new byte[16];
            ByteUtil.intToByteArray(ivBytes, 0, (int)counter);

            byte [] result = CryptographicEngine.Encrypt(ckey, message, ivBytes);
            return result;
        }


    }

    /// <summary>
    /// Decryption helpers
    /// </summary>
    public class Decrypt
    {
        public static byte[] aesCbcPkcs5(byte[] message, byte[] key, byte[] iv)
        {
            ISymmetricKeyAlgorithmProvider objAlg = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
            ICryptographicKey ckey = objAlg.CreateSymmetricKey(key);

            if (message.Length % objAlg.BlockLength != 0) throw new Exception("Invalid ciphertext length");
            
            byte [] result = CryptographicEngine.Decrypt(ckey, message, iv);
            return result;
        }

        public static byte[] aesCtr(byte[] message, byte[] key, uint counter)
        {
            ISymmetricKeyAlgorithmProvider objAlg = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
            ICryptographicKey ckey = objAlg.CreateSymmetricKey(key);

            byte[] ivBytes = new byte[16];
            ByteUtil.intToByteArray(ivBytes, 0, (int)counter);

            byte [] result = CryptographicEngine.Decrypt(ckey, message, ivBytes);
            return result;
        }
    }

    public static class CryptoHelper
    {
        /// <summary>
        /// TODO: dead code?
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
