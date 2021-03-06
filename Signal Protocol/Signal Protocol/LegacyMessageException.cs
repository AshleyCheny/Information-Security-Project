﻿/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
using System;

namespace libsignal
{
    public class LegacyMessageException : Exception
    {
        public LegacyMessageException()
        {
        }

        public LegacyMessageException(string s)
            : base(s)
        {
        }
    }
}
