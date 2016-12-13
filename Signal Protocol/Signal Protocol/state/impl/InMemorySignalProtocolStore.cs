/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
 using System.Collections.Generic;

namespace libsignal.state.impl
{
    public class InMemorySignalProtocolStore : SignalProtocolStore
    {

        private readonly InMemoryPreKeyStore preKeyStore = new InMemoryPreKeyStore();
        private readonly InMemorySessionStore sessionStore = new InMemorySessionStore();
        private readonly InMemorySignedPreKeyStore signedPreKeyStore = new InMemorySignedPreKeyStore();

        private readonly InMemoryIdentityKeyStore identityKeyStore;

        public InMemorySignalProtocolStore(IdentityKeyPair identityKeyPair, uint registrationId)
        {
            identityKeyStore = new InMemoryIdentityKeyStore(identityKeyPair, registrationId);
        }


        public IdentityKeyPair GetIdentityKeyPair()
        {
            return identityKeyStore.GetIdentityKeyPair();
        }


        public uint GetLocalRegistrationId()
        {
            return identityKeyStore.GetLocalRegistrationId();
        }


        public bool SaveIdentity(string name, IdentityKey identityKey)
        {
            identityKeyStore.SaveIdentity(name, identityKey);
            return true;
        }


        public bool IsTrustedIdentity(string name, IdentityKey identityKey)
        {
            return identityKeyStore.IsTrustedIdentity(name, identityKey);
        }


        public PreKeyRecord LoadPreKey(uint preKeyId)
        {
            return preKeyStore.LoadPreKey(preKeyId);
        }


        public void StorePreKey(uint preKeyId, PreKeyRecord record)
        {
            preKeyStore.StorePreKey(preKeyId, record);
        }


        public bool ContainsPreKey(uint preKeyId)
        {
            return preKeyStore.ContainsPreKey(preKeyId);
        }


        public void RemovePreKey(uint preKeyId)
        {
            preKeyStore.RemovePreKey(preKeyId);
        }


        public SessionRecord LoadSession(SignalProtocolAddress address)
        {
            return sessionStore.LoadSession(address);
        }


        public List<uint> GetSubDeviceSessions(string name)
        {
            return sessionStore.GetSubDeviceSessions(name);
        }


        public void StoreSession(SignalProtocolAddress address, SessionRecord record)
        {
            sessionStore.StoreSession(address, record);
        }


        public bool ContainsSession(SignalProtocolAddress address)
        {
            return sessionStore.ContainsSession(address);
        }


        public void DeleteSession(SignalProtocolAddress address)
        {
            sessionStore.DeleteSession(address);
        }


        public void DeleteAllSessions(string name)
        {
            sessionStore.DeleteAllSessions(name);
        }


        public SignedPreKeyRecord LoadSignedPreKey(uint signedPreKeyId)
        {
            return signedPreKeyStore.LoadSignedPreKey(signedPreKeyId);
        }


        public List<SignedPreKeyRecord> LoadSignedPreKeys()
        {
            return signedPreKeyStore.LoadSignedPreKeys();
        }


        public void StoreSignedPreKey(uint signedPreKeyId, SignedPreKeyRecord record)
        {
            signedPreKeyStore.StoreSignedPreKey(signedPreKeyId, record);
        }


        public bool ContainsSignedPreKey(uint signedPreKeyId)
        {
            return signedPreKeyStore.ContainsSignedPreKey(signedPreKeyId);
        }


        public void RemoveSignedPreKey(uint signedPreKeyId)
        {
            signedPreKeyStore.RemoveSignedPreKey(signedPreKeyId);
        }
    }
}
