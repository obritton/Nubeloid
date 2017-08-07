using UnityEngine;
using System.Collections;

public class PuzzleConfig : TextSearchBase
{
	public int cubesPerRow = 0;
	public int puzzleHeight = 0;
	public string frontImgName = "";
	public string sideImgName = "";
	public int fuzz = 0;
	public int freeze = 0;
	public int blitz = 0;
	
	public PuzzleConfig( string configStr )
	{
		string[] lines = configStr.Split();
		
		cubesPerRow = int.Parse(getValueForKey(lines, "CubesPerRow"));
		puzzleHeight = int.Parse(getValueForKey(lines, "PuzzleHeight"));
		frontImgName = getValueForKey(lines, "FrontImgName");
		sideImgName = getValueForKey(lines, "SideImgName");
		fuzz = int.Parse(getValueForKey(lines, "Static"));
		freeze = int.Parse(getValueForKey(lines, "Freeze"));
		blitz = int.Parse(getValueForKey(lines, "Blitz"));
	}
	
	public PuzzleConfig( int cubesPerRowIn, int puzzleHeightIn, string frontImgNameIn, string sideImgNameIn, int fuzzIn, int freezeIn, int blitzIn )
	{
		cubesPerRow = cubesPerRowIn;
		puzzleHeight = puzzleHeightIn;
		frontImgName = frontImgNameIn;
		sideImgName = sideImgNameIn;
		fuzz = fuzzIn;
		freeze = freezeIn;
		blitz = blitzIn;
	}
}
