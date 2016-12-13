/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
using System;

namespace libsignal
{
    public class SignalProtocolAddress
    {

        private readonly string name;
        private readonly uint deviceId;

        public SignalProtocolAddress(string name, uint deviceId)
        {
            this.name = name;
            this.deviceId = deviceId;
        }

        public string getName()
        {
            return name;
        }

        public uint getDeviceId()
        {
            return deviceId;
        }

        public override string ToString()
        {
            return name + ":" + deviceId;
        }

        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (!(other is SignalProtocolAddress)) return false;

            SignalProtocolAddress that = (SignalProtocolAddress)other;
            return name.Equals(that.name) && deviceId == that.deviceId;
        }


        public override int GetHashCode()
        {
            return name.GetHashCode() ^ (int)deviceId;
        }
    }
}
