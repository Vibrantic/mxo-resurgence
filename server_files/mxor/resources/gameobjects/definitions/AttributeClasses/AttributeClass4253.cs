using System;
using System.Runtime.InteropServices;

namespace mxor
{
	public class AttributeClass4253 :GameObject
	{
		public Attribute Position = new Attribute(24, "Position"); 
		public Attribute MoreInfoID = new Attribute(4, "MoreInfoID"); 
		public Attribute HalfExtents = new Attribute(12, "HalfExtents"); 
		public Attribute ReqAbilityID = new Attribute(4, "ReqAbilityID"); 
		public Attribute InfoID = new Attribute(4, "InfoID"); 
		public Attribute Orientation = new Attribute(16, "Orientation"); 


		 public AttributeClass4253(string name,UInt16 _goid)
		: base(6, 2, name, _goid, 0xFFFFFFFF)
	        {
			AddAttribute(ref Position, 0, -1); 
			AddAttribute(ref MoreInfoID, 1, 0); 
			AddAttribute(ref HalfExtents, 2, -1); 
			AddAttribute(ref ReqAbilityID, 3, -1); 
			AddAttribute(ref InfoID, 4, 1); 
			AddAttribute(ref Orientation, 5, -1); 

		}

	}

}