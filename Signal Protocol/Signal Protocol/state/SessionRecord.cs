/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
using System.Collections.Generic;
using System.Linq;
using static libsignal.state.StorageProtos;

namespace libsignal.state
{
    /**
 * A SessionRecord encapsulates the state of an ongoing session.
 *
 * @author Moxie Marlinspike
 */
    public class SessionRecord
    {

        private static int ARCHIVED_STATES_MAX_LENGTH = 40;

        private SessionState sessionState = new SessionState();
        private LinkedList<SessionState> previousStates = new LinkedList<SessionState>();
        private bool fresh = false;

        public SessionRecord()
        {
            fresh = true;
        }

        public SessionRecord(SessionState sessionState)
        {
            this.sessionState = sessionState;
            fresh = false;
        }

        public SessionRecord(byte[] serialized)
        {
            RecordStructure record = RecordStructure.ParseFrom(serialized);
            sessionState = new SessionState(record.CurrentSession);
            fresh = false;

            foreach (SessionStructure previousStructure in record.PreviousSessionsList)
            {
                previousStates.AddLast(new SessionState(previousStructure));
            }
        }

        public bool hasSessionState(uint version, byte[] aliceBaseKey)
        {
            if (sessionState.getSessionVersion() == version &&
                Enumerable.SequenceEqual(aliceBaseKey, sessionState.getAliceBaseKey()))
            {
                return true;
            }

            foreach (SessionState state in previousStates)
            {
                if (state.getSessionVersion() == version &&
                    Enumerable.SequenceEqual(aliceBaseKey, state.getAliceBaseKey()))
                {
                    return true;
                }
            }

            return false;
        }

        public SessionState getSessionState()
        {
            return sessionState;
        }

        /**
         * @return the list of all currently maintained "previous" session states.
         */
        public LinkedList<SessionState> getPreviousSessionStates()
        {
            return previousStates;
        }


        public bool isFresh()
        {
            return fresh;
        }

        /**
         * Move the current {@link SessionState} into the list of "previous" session states,
         * and replace the current {@link org.whispersystems.libsignal.state.SessionState}
         * with a fresh reset instance.
         */
        public void archiveCurrentState()
        {
            promoteState(new SessionState());
        }

        public void promoteState(SessionState promotedState)
        {
            previousStates.AddFirst(sessionState);
            sessionState = promotedState;

            if (previousStates.Count > ARCHIVED_STATES_MAX_LENGTH)
            {
                previousStates.RemoveLast();
            }
        }

        public void setState(SessionState sessionState)
        {
            this.sessionState = sessionState;
        }

        /**
         * @return a serialized version of the current SessionRecord.
         */
        public byte[] serialize()
        {
            List<SessionStructure> previousStructures = new List<SessionStructure>();

            foreach (SessionState previousState in previousStates)
            {
                previousStructures.Add(previousState.getStructure());
            }

            RecordStructure record = RecordStructure.CreateBuilder()
                                                    .SetCurrentSession(sessionState.getStructure())
                                                    .AddRangePreviousSessions(previousStructures)
                                                    .Build();

            return record.ToByteArray();
        }

    }
}
