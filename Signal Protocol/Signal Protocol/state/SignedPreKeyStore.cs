﻿/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
 using System.Collections.Generic;

namespace libsignal.state
{
    public interface SignedPreKeyStore
    {


        /**
         * Load a local SignedPreKeyRecord.
         *
         * @param signedPreKeyId the ID of the local SignedPreKeyRecord.
         * @return the corresponding SignedPreKeyRecord.
         * @throws InvalidKeyIdException when there is no corresponding SignedPreKeyRecord.
         */
        SignedPreKeyRecord LoadSignedPreKey(uint signedPreKeyId);

        /**
         * Load all local SignedPreKeyRecords.
         *
         * @return All stored SignedPreKeyRecords.
         */
        List<SignedPreKeyRecord> LoadSignedPreKeys();

        /**
         * Store a local SignedPreKeyRecord.
         *
         * @param signedPreKeyId the ID of the SignedPreKeyRecord to store.
         * @param record the SignedPreKeyRecord.
         */
        void StoreSignedPreKey(uint signedPreKeyId, SignedPreKeyRecord record);

        /**
         * @param signedPreKeyId A SignedPreKeyRecord ID.
         * @return true if the store has a record for the signedPreKeyId, otherwise false.
         */
        bool ContainsSignedPreKey(uint signedPreKeyId);

        /**
         * Delete a SignedPreKeyRecord from local storage.
         *
         * @param signedPreKeyId The ID of the SignedPreKeyRecord to remove.
         */
        void RemoveSignedPreKey(uint signedPreKeyId);
    }
}
