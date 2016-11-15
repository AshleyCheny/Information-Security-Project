using static PCLCrypto.WinRTCrypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLCrypto;

namespace Signal_Protocol.util
{
    class HMAC
    {
    }

    public class Sign
    {
        public static byte[] sha256sum(byte[] key, byte[] message)
        {
            IMacAlgorithmProvider provider = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithm.HmacSha256);
            ICryptographicKey hmacKey = provider.CreateKey(key);
            byte[] hmac = CryptographicEngine.Sign(hmacKey, message);
            return hmac;
        }
    }
}
