using UnityEngine;
using System.Collections;

public class WinManager : MonoBehaviour {

	public Transform cubeloid;
	
	public enum RowState { Scattered=0, AllCubesInRow, AllCubesRotated, AllCubesRotAndPositioned }
	
	private PuzzleFactory pFactoryComponent;
	
	void Start()
	{
		pFactoryComponent = GetComponent<PuzzleFactory>();
	}
	
	public RowState getRowState( int row )
	{		
		RowState returnState = RowState.Scattered;
		
		float[] heightArray = PuzzleFactory.base4PosArr;
		switch( pFactoryComponent.cubesPerRow ){
		default:
		case 2: heightArray = PuzzleFactory.base4PosArr;	break;
		case 3:	heightArray = PuzzleFactory.base9PosArr;	break;			
		case 4:	heightArray = PuzzleFactory.base16PosArr;	break;
		}
		
		float targetHeight = heightArray[row];
		
		int totalInRow = cubeloid.childCount;
		int totalRotated = 0;
		int totalPosed = 0;
		int cubesPerRow = pFactoryComponent.cubesPerRow;
		int firstRot = -1;
		int currentRot = -1;
		
		for( int i = 0; i < cubeloid.childCount; i++ )
		{
			Transform cube = cubeloid.GetChild(i);
			CubeData cubeData = cube.GetComponent<CubeData>();
			
			//Verify row has all cubes
			if( cubeData.row == row ^ Mathf.Abs( cube.transform.localPosition.y - targetHeight) < 0.2f )
				totalInRow--;
			
			//Verify rotation
			int rot = (int)cube.localRotation.eulerAngles.y;
			
			while( rot < 0 ) rot += 360;
			while( rot >= 180 ) rot -= 180;
			
			print( "row: " + i + ": " + rot );
			
			if( cubeData.row == row )
			{
				if( firstRot < 0 )
					firstRot = rot;
				
				if( Mathf.Abs( firstRot - rot ) < 5 )
						totalRotated++;
			}
			
			//Verify all cubes are posed
			bool flipped = Mathf.Abs( firstRot ) > 5;
			if( cubeData.row == row &&
				Mathf.Abs( cube.transform.localPosition.x - (flipped ? cubeData.zPos : cubeData.xPos)) < 0.2f &&
				Mathf.Abs( cube.transform.localPosition.z - (flipped ? cubeData.xPos : cubeData.zPos)) < 0.2f )
				totalPosed++;
		}
		//Track first time for all of the above
		//All rotation should mnatch rotation of first row
		
		if( totalInRow == cubeloid.childCount )
		{
			if( totalRotated == cubesPerRow )
			{
				if( totalPosed == cubesPerRow )
					return RowState.AllCubesRotAndPositioned;
				
				return RowState.AllCubesRotated;
			}
			return RowState.AllCubesInRow;
		}
		
		return RowState.Scattered;
	}
}
