/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
 using System;

namespace TextSecure.libsignal
{
    class InvalidMacException : Exception
    {

        public InvalidMacException(string detailMessage)
            :base(detailMessage)
        {
        }

        public InvalidMacException(Exception exception)
            :base(exception.Message)
        {

        }
    }
}
