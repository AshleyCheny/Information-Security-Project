﻿/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
 namespace libsignal
{
    public interface DecryptionCallback
    {
        void handlePlaintext(byte[] plaintext);
    }
}
