/**
 * Copyright (C) 2013-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */

using System;

namespace libsignal.ecc
{
    public interface ECPublicKey : IComparable
    {
        byte[] serialize();

        int getType();
    }
}
