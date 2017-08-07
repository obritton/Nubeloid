using UnityEngine;
using System.Collections;

public class LevelConfig : TextSearchBase
{
	public int totalPuzzles = 0;
	public int currentPuzzle = 0;
	
	public int timer = 0;
	public int currentTimer = 0;
	public PuzzleConfig[] puzzleConfigArr;
	
	public LevelConfig( TextAsset configFile )
	{
		buildConfigObjects( configFile );
	}
	
	public LevelConfig( PuzzleConfig onePuzzle )
	{
		totalPuzzles = 1;
		
		puzzleConfigArr = new PuzzleConfig[1];
		puzzleConfigArr[0] = onePuzzle;
	}
	
	void buildConfigObjects( TextAsset configFile )
	{
		string text = configFile.text;
		string[] puzzleStrArr = text.Split("*"[0]);
		totalPuzzles = puzzleStrArr.Length - 1;
		
		string timerStr = getValueForKey(puzzleStrArr[0].Split(), "Timer");
		if( timerStr != "" )
			timer = int.Parse(timerStr);
		
		//Build puzzles
		puzzleConfigArr = new PuzzleConfig[totalPuzzles];
		for( int i = 0; i < totalPuzzles; i++ )
			puzzleConfigArr[i] = new PuzzleConfig( puzzleStrArr[i+1] );
	}
}
