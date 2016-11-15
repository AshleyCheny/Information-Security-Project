using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Signal_Protocol
{
    public class InvalidKeyException : Exception
    {
        public InvalidKeyException() { }

        public InvalidKeyException(String detailMessage) : base(detailMessage)
        {

        }

        public InvalidKeyException(Exception throwable) : base(throwable.Message)
        {

        }

        public InvalidKeyException(String detailMessage, Exception throwable) : base(detailMessage, throwable)
        {

        }
    }
}
