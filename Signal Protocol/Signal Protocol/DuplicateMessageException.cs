using System;

namespace libsignal
{
    public class DuplicateMessageException : Exception
    {
        public DuplicateMessageException(string s)
            : base(s)
        {
        }
    }
}
