using System;
using System.Runtime.InteropServices;

namespace mxor
{
	public class AttributeClass981 :GameObject
	{
		public Attribute Orientation = new Attribute(16, "Orientation"); 
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 


		 public AttributeClass981(string name,UInt16 _goid)
		: base(3, 0, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Orientation, 0, -1); 
			AddAttribute(ref Position, 1, -1); 
			AddAttribute(ref HalfExtents, 2, -1); 

		}

	}

}