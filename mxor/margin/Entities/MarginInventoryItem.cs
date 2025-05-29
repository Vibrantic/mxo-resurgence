using System;
using System.Collections;
using System.Text;

namespace mxor.margin.Entities
{
    public class MarginInventoryItem
    {

        private uint itemID;
        private ushort amount;
        private ushort purity;
        private ushort slot;
        public MarginInventoryItem()
        {

        }

        public void SetItemID(uint itemID)
        {
            this.itemID = itemID;
        }

        public uint GetItemID()
        {
            return itemID;
        }

        public void SetAmount(ushort count)
        {
            amount = count;
        }

        public ushort GetAmount()
        {
            return amount;
        }

        public void SetPurity(ushort purity)
        {
            this.purity = purity;
        }

        public ushort GetPurity()
        {
            return purity;
        }

        public void SetSlot(ushort slot)
        {
            this.slot = slot;
        }

        public ushort GetSlot()
        {
            return slot;
        }
    }
}
