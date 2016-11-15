namespace Signal_Protocol.kdf
{
    internal class HKDFv3 : HKDF
    {
        override protected int getIterationStartOffset()
        {
            return 1;
        }
    }
}