using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace mxor.margin.Entities
{
    public class MarginAbilityItem
    {

        public int AbilityID;
        public ushort level;
        public ushort slot;
        public bool isLoaded;

        public MarginAbilityItem()
        {

        }

        public void SetAbilityID(int AbilityID)
        {
            this.AbilityID = AbilityID;
        }

        public int GetAbilityID()
        {
            return AbilityID;
        }

        public void SetLevel(ushort level)
        {
            this.level = level;
        }

        public ushort GetLevel()
        {
            return level;
        }

        public void SetSlot(ushort slot)
        {
            this.slot = slot;
        }

        public ushort GetSlot()
        {
            return slot;
        }

        public void SetLoaded(bool isLoaded)
        {
            this.isLoaded = isLoaded;
        }

        public bool GetLoaded()
        {
            return isLoaded;
        }



    }
}
