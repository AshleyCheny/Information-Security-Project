using org.whispersystems.curve25519;
using System;
using System.Collections.Generic;
using System.Linq;      
using System.Text;
using System.Threading.Tasks;

namespace Signal_Protocol.encryptioncurve
{
    public class Curve
    {
        public const int DJB_TYPE = 0x05;

        public static bool isNative()
        {
            return Curve25519.getInstance(Curve25519ProviderType.BEST).isNative();
        }
    }
}
