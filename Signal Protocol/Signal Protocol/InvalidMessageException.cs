/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace libsignal
{
    public class InvalidMessageException : Exception
    {

        public InvalidMessageException() { }

        public InvalidMessageException(string detailMessage)
                        : base(detailMessage)
        {

        }

        public InvalidMessageException(Exception exception)
                        : base(exception.Message)
        {

        }

        public InvalidMessageException(string detailMessage, Exception exception)
                        : base(detailMessage, exception)
        {

        }

        public InvalidMessageException(string detailMessage, List<Exception> exceptions)
                        : base(string.Join(",", exceptions.Select(x => x.Message).ToArray()))
        {

        }
        public InvalidMessageException(string detailMessage, LinkedList<Exception> exceptions)
                        : base(string.Join(",", exceptions.Select(x => x.Message).ToArray()))
        {

        }
    }
}
