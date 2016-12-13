/**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */
namespace libsignal.kdf
{
    public class HKDFv3 : HKDF
    {
        protected override int getIterationStartOffset()
        {
            return 1;
        }
    }
}
