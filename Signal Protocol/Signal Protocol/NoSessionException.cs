/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
using System;

namespace libsignal
{
    public class NoSessionException : Exception
    {
        public NoSessionException()
        {
        }

        public NoSessionException(string s)
            : base(s)
        {
        }

        public NoSessionException(Exception exception)
           : base(exception.Message)
        {
        }
    }
}
