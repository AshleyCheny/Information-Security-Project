/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
using System;

namespace libsignal.exceptions
{
    public class UntrustedIdentityException : Exception
    {
        private readonly string name;
        private readonly IdentityKey key;

        public UntrustedIdentityException(string name, IdentityKey key)
        {
            this.name = name;
            this.key = key;
        }

        public IdentityKey getUntrustedIdentity()
        {
            return key;
        }

        public string getName()
        {
            return name;
        }
    }
}