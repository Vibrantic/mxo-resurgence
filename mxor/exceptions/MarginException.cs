using System;

namespace mxor.exceptions
{
	
	
	public class MarginException: Exception
	{
		
		// Autopass errorString to Exception class
		public MarginException(string errorString):base(errorString){

		}
	
		// We want a customized ToString method, so override
		public override string ToString(){
			return "Margin error: "+ Message;
		}

	}
}
