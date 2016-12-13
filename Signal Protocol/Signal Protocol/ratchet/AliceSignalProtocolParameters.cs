﻿/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */

using libsignal.ecc;
using Strilanc.Value;
using System;

namespace libsignal.ratchet
{
    public class AliceSignalProtocolParameters
    {

        private readonly IdentityKeyPair ourIdentityKey;
        private readonly ECKeyPair ourBaseKey;

        private readonly IdentityKey theirIdentityKey;
        private readonly ECPublicKey theirSignedPreKey;
        private readonly May<ECPublicKey> theirOneTimePreKey;
        private readonly ECPublicKey theirRatchetKey;

        private AliceSignalProtocolParameters(IdentityKeyPair ourIdentityKey, ECKeyPair ourBaseKey,
                                       IdentityKey theirIdentityKey, ECPublicKey theirSignedPreKey,
                                       ECPublicKey theirRatchetKey, May<ECPublicKey> theirOneTimePreKey)
        {
            this.ourIdentityKey = ourIdentityKey;
            this.ourBaseKey = ourBaseKey;
            this.theirIdentityKey = theirIdentityKey;
            this.theirSignedPreKey = theirSignedPreKey;
            this.theirRatchetKey = theirRatchetKey;
            this.theirOneTimePreKey = theirOneTimePreKey;

            if (ourIdentityKey == null || ourBaseKey == null || theirIdentityKey == null ||
                theirSignedPreKey == null || theirRatchetKey == null || theirOneTimePreKey == null)
            {
                throw new Exception("Null values!");
            }
        }

        public IdentityKeyPair getOurIdentityKey()
        {
            return ourIdentityKey;
        }

        public ECKeyPair getOurBaseKey()
        {
            return ourBaseKey;
        }

        public IdentityKey getTheirIdentityKey()
        {
            return theirIdentityKey;
        }

        public ECPublicKey getTheirSignedPreKey()
        {
            return theirSignedPreKey;
        }

        public May<ECPublicKey> getTheirOneTimePreKey()
        {
            return theirOneTimePreKey;
        }

        public static Builder newBuilder()
        {
            return new Builder();
        }

        public ECPublicKey getTheirRatchetKey()
        {
            return theirRatchetKey;
        }

        public class Builder
        {
            private IdentityKeyPair ourIdentityKey;
            private ECKeyPair ourBaseKey;

            private IdentityKey theirIdentityKey;
            private ECPublicKey theirSignedPreKey;
            private ECPublicKey theirRatchetKey;
            private May<ECPublicKey> theirOneTimePreKey;

            public Builder setOurIdentityKey(IdentityKeyPair ourIdentityKey)
            {
                this.ourIdentityKey = ourIdentityKey;
                return this;
            }

            public Builder setOurBaseKey(ECKeyPair ourBaseKey)
            {
                this.ourBaseKey = ourBaseKey;
                return this;
            }

            public Builder setTheirRatchetKey(ECPublicKey theirRatchetKey)
            {
                this.theirRatchetKey = theirRatchetKey;
                return this;
            }

            public Builder setTheirIdentityKey(IdentityKey theirIdentityKey)
            {
                this.theirIdentityKey = theirIdentityKey;
                return this;
            }

            public Builder setTheirSignedPreKey(ECPublicKey theirSignedPreKey)
            {
                this.theirSignedPreKey = theirSignedPreKey;
                return this;
            }

            public Builder setTheirOneTimePreKey(May<ECPublicKey> theirOneTimePreKey)
            {
                this.theirOneTimePreKey = theirOneTimePreKey;
                return this;
            }

            public AliceSignalProtocolParameters create()
            {
                return new AliceSignalProtocolParameters(ourIdentityKey, ourBaseKey, theirIdentityKey,
                                                  theirSignedPreKey, theirRatchetKey, theirOneTimePreKey);
            }
        }
    }
}
