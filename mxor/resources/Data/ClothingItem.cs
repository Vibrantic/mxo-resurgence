﻿﻿using System;
using System.Collections.Generic;

using System.Text;

namespace mxor
{
    class ClothingItem
    {
        private UInt16 GoidDecimal;
        private string ClothesType;
        private string ShortName;
        private string ItemName;
        private uint ModelId;
        private uint ColorId;

        public ClothingItem(){

        }

        public void setGoidDecimal(UInt16 GOdec)
        {
            this.GoidDecimal = GOdec;
        }

        public UInt16 getGoidDecimal()
        {
            return this.GoidDecimal;
        }


        public void setClothesType(string type)
        {
            this.ClothesType = type;
        }

        public string getClothesType()
        {
            return this.ClothesType;
        }

        public void setShortName(string name)
        {
            this.ShortName = name;
        }

        public string getShortName()
        {
            return this.ShortName;
        }

        public void setItemName(string name)
        {
            this.ItemName = name;
        }

        public string getItemName()
        {
            return this.ItemName;
        }

        public void setModelId(uint id)
        {
            this.ModelId = id;
        }

        public uint getModelId()
        {
            return this.ModelId;
        }

        public void setColorId(uint id)
        {
            this.ColorId = id;
        }

        public uint getColorId()
        {
            return this.ColorId;
        }


    }
}
