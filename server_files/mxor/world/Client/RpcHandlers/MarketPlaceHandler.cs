﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using mxor.shared;

namespace mxor{

    public class MarketPlaceHandler{

        public void processMarketplaceList(ref byte[] packet)
        {
            
            // ToDo: Load Data from Database for each goCategory 
            ServerPackets pak = new ServerPackets();
            pak.sendMarketPlaceList(Store.currentClient);
        }
    }
}
