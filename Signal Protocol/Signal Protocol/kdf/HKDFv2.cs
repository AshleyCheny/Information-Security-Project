namespace Signal_Protocol.kdf
{
    internal class HKDFv2 : HKDF
    {
        override protected int getIterationStartOffset()
        {
            return 0;
        }
    }
}