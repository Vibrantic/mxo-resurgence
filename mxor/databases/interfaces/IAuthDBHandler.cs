using mxor.auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace mxor.databases.interfaces{
    public interface IAuthDBHandler
    {
        bool FetchWorldList(ref WorldList wl);
    }
}
