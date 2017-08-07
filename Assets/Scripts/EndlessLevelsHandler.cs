using UnityEngine;
using System.Collections;

public class EndlessLevelsHandler {
	
	public static int[] cubesPerRowRange = {0, 0};
	public static int[] puzzleHeightRange = {0, 0};
	public static int[] fuzzRange = {0, 0};
	public static int[] freezeRange = {0, 0};
	public static int[] blitzRange = {0, 0};
	
	static string[] picsArr = {"1x1a", "1x1b", "1x1c", "1x1d", "1x1e", "1x1f", "1x1g", "1x1h", "1x1i", "1x1j", "1x1k", "1x1l", "1x1m", "1x1n", "1x1o", "1x1p", "1x1q", "1x1r", "1x1s"};
	
	public static void buildRandomLevel()
	{
		int cubesPerRow = Random.Range(2,4+1);
		int height = Random.Range(1,cubesPerRow+1);
		
		string pic1 = EndlessLevelsHandler.picsArr[Random.Range(0,EndlessLevelsHandler.picsArr.Length)];
		string pic2 = pic1;
		while( pic2 == pic1 )
			pic2 = EndlessLevelsHandler.picsArr[Random.Range(0,EndlessLevelsHandler.picsArr.Length)];
		
		int totalFuzz = Random.value > 0.5f ? Random.Range( 0, 5 ) : 0;
		
		LevelConfig level = new LevelConfig( new PuzzleConfig( cubesPerRow, height, pic1, pic2, totalFuzz, 0, 0));
		PuzzleFactory.levelConfig = level;
	}
}
