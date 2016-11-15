using System;

namespace Signal_Protocol.ecc
{
    public interface ECPublicKey : IComparable
    {
        //public static int KEY_SIZE = 33;

        byte[] serialize();

        int getType();
    }
}