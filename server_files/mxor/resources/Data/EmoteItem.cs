﻿using System;

namespace mxor
{
    public class EmoteItem
    {
        public UInt32 emoteIDLong;
        public byte emoteShortID;

        public EmoteItem(UInt32 _emoteIDLong, byte _emoteShortID)
        {
            this.emoteIDLong = _emoteIDLong;
            this.emoteShortID = _emoteShortID;

        }
    }
}
