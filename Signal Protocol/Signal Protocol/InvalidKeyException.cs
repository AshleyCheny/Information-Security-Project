/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
 using System;

namespace libsignal
{
    public class InvalidKeyException : Exception
    {

        public InvalidKeyException() { }

        public InvalidKeyException(string detailMessage)
            : base(detailMessage)
        {

        }

        public InvalidKeyException(Exception exception)
            : base(exception.Message)
        {

        }

        public InvalidKeyException(string detailMessage, Exception exception)
            : base(detailMessage, exception)
        {

        }
    }
}
