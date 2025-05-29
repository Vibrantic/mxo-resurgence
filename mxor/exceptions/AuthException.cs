using System;

namespace mxor.exceptions
{
	
	
	public class AuthException: Exception
	{

		// Autopass errorString to Exception class
		public AuthException(string errorString):base(errorString){

		}
	
		// We want a customized ToString method, so override
		public override string ToString(){
			return "Auth error: "+ Message;
		}

	}
}
