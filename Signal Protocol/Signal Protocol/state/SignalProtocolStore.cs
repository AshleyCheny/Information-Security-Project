/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
 namespace libsignal.state
{
    public interface SignalProtocolStore : IdentityKeyStore, PreKeyStore, SessionStore, SignedPreKeyStore
    {
    }

}
