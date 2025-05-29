using System.Collections.Generic;

namespace mxor.world.scripting{

	public interface IScript{

		Dictionary<int,string> Register();
		bool Test();
	}
}

