/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
 using System;

namespace libsignal
{
    public class InvalidVersionException : Exception
    {
        public InvalidVersionException()
        {
        }

        public InvalidVersionException(string detailMessage)
            : base (detailMessage)
        {
        }
    }
}
