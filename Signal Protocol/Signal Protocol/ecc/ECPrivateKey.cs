namespace Signal_Protocol.ecc
{
    public interface ECPrivateKey
    {
        byte[] serialize();
        int getType();
    }
}