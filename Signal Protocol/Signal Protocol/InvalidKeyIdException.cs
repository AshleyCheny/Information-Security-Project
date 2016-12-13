/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
 using System;

namespace libsignal
{
    public class InvalidKeyIdException : Exception
    {
        public InvalidKeyIdException(string detailMessage)
            :base(detailMessage)
        {
        }

        public InvalidKeyIdException(Exception exception)
            :base(exception.Message)
        {
        }
    }
}
