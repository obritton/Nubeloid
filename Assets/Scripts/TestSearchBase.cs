using UnityEngine;
using System.Collections;

public class TextSearchBase
{
	protected string getValueForKey( string[] lines, string key )
	{
		string keyStr = search ( lines, key );
		string[] keyVal = keyStr.Split ( ":"[0] );
		
		if( keyVal.Length > 1 )
			return keyVal[1];
		
		return "";
	}
	
	string search( string[] stringArr, string searchTerm )
	{
		foreach( string str in stringArr )
			if( str.Contains(searchTerm))
				return str;
		
		return "";
	}
}
