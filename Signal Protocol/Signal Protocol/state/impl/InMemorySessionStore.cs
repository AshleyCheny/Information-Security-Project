/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
using System;
using System.Collections.Generic;

namespace libsignal.state.impl
{
    public class InMemorySessionStore : SessionStore
	{

		static object Lock = new object();

		private IDictionary<SignalProtocolAddress, byte[]> sessions = new Dictionary<SignalProtocolAddress, byte[]>();

		public InMemorySessionStore() { }
        
		public SessionRecord LoadSession(SignalProtocolAddress remoteAddress)
		{
			try
			{
				if (ContainsSession(remoteAddress))
				{
					byte[] session;
					sessions.TryGetValue(remoteAddress, out session);

					return new SessionRecord(session);
				}
				else
				{
					return new SessionRecord();
				}
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}


		public List<uint> GetSubDeviceSessions(string name)
		{
			List<uint> deviceIds = new List<uint>();

			foreach (SignalProtocolAddress key in sessions.Keys)
			{
				if (key.getName().Equals(name) &&
					key.getDeviceId() != 1)
				{
					deviceIds.Add(key.getDeviceId());
				}
			}

			return deviceIds;
		}


		public void StoreSession(string name, uint deviceID, byte[] record)
		{
            sessions[new SignalProtocolAddress(name, deviceID)] = record;
		}

        public void StoreSession(SignalProtocolAddress address, SessionRecord record)
        {
            sessions[address] = record.serialize();
        }

        public bool ContainsSession(SignalProtocolAddress address)
		{
			return sessions.ContainsKey(address);
		}


		public void DeleteSession(SignalProtocolAddress address)
		{
			sessions.Remove(address);
		}

        public List<Session> GetAllSessions()
        {
            List<Session> OutSession = new List<Session>();
            foreach (KeyValuePair<SignalProtocolAddress, byte[]> item in sessions)
            {
                Session s = new Session(); 
                s.Name = item.Key.getName();
                s.DeviceID = item.Key.getDeviceId();
                s.array = item.Value;
                OutSession.Add(s);
            }
            return OutSession;
        }

        public void DeleteAllSessions(string name)
		{
			foreach (SignalProtocolAddress key in sessions.Keys)
			{
				if (key.getName().Equals(name))
				{
					sessions.Remove(key);
				}
			}
		}
	}
}
