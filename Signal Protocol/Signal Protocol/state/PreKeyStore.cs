/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
namespace libsignal.state
{
    /**
     * An interface describing the local storage of {@link PreKeyRecord}s.
     *
     * @author
     */
    public interface PreKeyStore
    {

        /**
         * Load a local PreKeyRecord.
         *
         * @param preKeyId the ID of the local PreKeyRecord.
         * @return the corresponding PreKeyRecord.
         * @throws InvalidKeyIdException when there is no corresponding PreKeyRecord.
         */
        PreKeyRecord LoadPreKey(uint preKeyId);

        /**
         * Store a local PreKeyRecord.
         *
         * @param preKeyId the ID of the PreKeyRecord to store.
         * @param record the PreKeyRecord.
         */
        void StorePreKey(uint preKeyId, PreKeyRecord record);

        /**
         * @param preKeyId A PreKeyRecord ID.
         * @return true if the store has a record for the preKeyId, otherwise false.
         */
         bool ContainsPreKey(uint preKeyId);

        /**
         * Delete a PreKeyRecord from local storage.
         *
         * @param preKeyId The ID of the PreKeyRecord to remove.
         */
        void RemovePreKey(uint preKeyId);

    }
}
