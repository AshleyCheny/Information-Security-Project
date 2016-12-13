/**
 * Copyright (C) 2013-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
namespace libsignal.ecc
{
    public interface ECPrivateKey
    {
        byte[] serialize();
        int getType();
    }
}
