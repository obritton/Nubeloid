using UnityEngine;
using System.Collections;

public class PuzzleFactory : MonoBehaviour {

	public int cubesPerRow = 2;
	public int puzzleHeight = 1;
	public int totalStaticFaces = 0;
	public Texture2D overrideFront;
	public Texture2D overrideSide;
	public bool mixEm = true;
	public GameObject cubePrefab;
	public Transform cubeloid;
	public Transform reflection;
	private GameObject[] cubes;
	private GameObject[] reflections;
	public Transform floor;
	
	public Material materialFront;
	public Material materialSide;
	
	private int maxCubes = 0;
	private int totalCubes;
	
	public static LevelConfig levelConfig;
	
	void Awake (){
		createPuzzle();
	}
	
	public void createPuzzle()
	{
		if( levelConfig != null )
		{
			setParamsFromConfig();
			int timerLength = levelConfig.timer;
			int currentTimer = levelConfig.currentTimer;
			
			if( timerLength > 0 )
			{
				TimerManager tm = gameObject.GetComponent<TimerManager>();
				tm.totalTime = timerLength;
				if( currentTimer > 0 )
					tm.currentTime = currentTimer;
				else
					tm.currentTime = timerLength;
			}
			else
				Destroy( GetComponent<TimerManager>());
		}
		createCubes();
		setupCameraPosition();
		applyTexturedFaces();
		createWinPositions();
		
		GetComponent<FuzzBlock>().addStatic(totalStaticFaces);
		
		if( mixEm )
			mixUpCubes();
		
		createReflections();
	}
	
	void setParamsFromConfig()
	{
		PuzzleConfig puzzConfig = levelConfig.puzzleConfigArr[levelConfig.currentPuzzle++];
		
		cubesPerRow = puzzConfig.cubesPerRow;
		puzzleHeight = puzzConfig.puzzleHeight;
		totalStaticFaces = puzzConfig.fuzz;
		overrideFront = Resources.Load( "levelconfigs/" + puzzConfig.frontImgName ) as Texture2D;
		overrideSide = Resources.Load( "levelconfigs/" + puzzConfig.sideImgName ) as Texture2D;
	}
	
	void createCubes()
	{
		cubes = new GameObject[puzzleHeight * cubesPerRow];
		for( int r = 0; r < puzzleHeight; r++ )
			for( int c = 0; c < cubesPerRow; c++ )
			{
				int i = (r * cubesPerRow) + c;
				cubes[i] = Instantiate( cubePrefab, Vector3.zero, Quaternion.identity ) as GameObject;
				cubes[i].transform.parent = cubeloid;
			}
		
		maxCubes = cubesPerRow * cubesPerRow;
		totalCubes = cubesPerRow * puzzleHeight;
		
		float[] positionArr = base4PosArr;

		switch( maxCubes ){
		case 4:
			positionArr = base4PosArr;
			break;
		case 9:
			positionArr = base9PosArr;
			break;
		case 16:
			positionArr = base16PosArr;
			break;
		}
		setPositionsBase( positionArr );	
	}
	
	void setupCameraPosition()
	{
		//Map max cubes 4, 9, 16 onto camera distances -8, -12, -16
		Vector3 camPos = Camera.main.transform.position;
		camPos.z = Mathf.Sqrt(maxCubes) * - 3 + 1.0f;
		Camera.main.transform.position = camPos;
	}
	void applyTexturedFaces(){
		
		int frontI = Random.Range(1,16);
		int sideI = frontI;
		while( sideI == frontI )
			sideI = Random.Range(1,16);
		
		Texture2D frontTex = (Texture2D)Resources.Load("FruitTextures/" + frontI);
		materialFront.SetTexture( "_MainTex", frontTex );
		Texture2D sideTex = (Texture2D)Resources.Load("FruitTextures/" + sideI);
		materialSide.SetTexture( "_MainTex", sideTex );
		
		
		materialFront.SetTexture( "_MainTex", overrideFront );
		materialSide.SetTexture( "_MainTex", overrideSide );
		
		GameObject faceBundle;
		switch( maxCubes ){
		case 4:
		default:
			cubeloid.tag = "quadraloid";
			faceBundle = (GameObject)Resources.Load("Quad_Faces");
			break;
		case 9:
			cubeloid.tag = "novaloid";
			faceBundle = (GameObject)Resources.Load("Nova_Faces");
			break;
		case 16:
			cubeloid.tag = "hexaloid";
			faceBundle = (GameObject)Resources.Load("Hex_Faces");
			break;
		}
		
		GameObject[] faces = faceBundle.GetComponent<FaceIndexer>().faces;
		
		foreach( GameObject cube in cubes ){
			GameObject front, back, left, right ;
			int fIndex = 0;
			int sIndex = 0;
			Vector3 pos = cube.transform.localPosition;
			
			switch( maxCubes ){
			case 4:
				pos *= 0.5f;
				fIndex = (pos.x < 0 ? 0 : 1) + (pos.y < 0 ? 2 : 0);
				sIndex = (pos.z < 0 ? 0 : 1) + (pos.y < 0 ? 2 : 0);
				break;
			case 9:
				fIndex = 3 * (1 - (int)pos.y) + 1 + (int)pos.x;
				sIndex = 3 * (1 - (int)pos.y) + 1 + (int)pos.z;
				break;
			case 16:
				pos *= 2;
				fIndex = (3-(((int)pos.y+3)/2)) * 4 + 3+(((int)pos.x-3)/2);
				sIndex = (3-(((int)pos.y+3)/2)) * 4 + 3+(((int)pos.z-3)/2);
				break;
			}
			
			front = Instantiate(faces[fIndex], cube.transform.position, cube.transform.rotation) as GameObject;
			front.tag = "frontFace";
			front.transform.parent = cube.transform;
			front.GetComponent<Renderer>().material = materialFront;
			front.transform.Rotate( -90, 0, 0 );
			front.transform.Translate( 0, 0.5f, 0);
			front.name = "front";
			
			back = Instantiate(faces[fIndex], cube.transform.position, cube.transform.rotation) as GameObject;
			back.transform.parent = cube.transform;
			back.GetComponent<Renderer>().material = materialFront;
			back.transform.Rotate( -90, 0, 180 );
			back.transform.Translate( 0, 0.5f, 0);
			back.name = "back";
			
			right = Instantiate( faces[sIndex], cube.transform.position, cube.transform.rotation) as GameObject;
			right.tag = "rightFace";
			right.transform.parent = cube.transform;
			right.GetComponent<Renderer>().material = materialSide;
			right.transform.Rotate( -90, 0, -90 );
			right.transform.Translate( 0, 0.5f, 0 );
			right.name = "right";
			
			left = Instantiate( faces[sIndex], cube.transform.position, cube.transform.rotation) as GameObject;
			left.transform.parent = cube.transform;
			left.GetComponent<Renderer>().material = materialSide;
			left.transform.Rotate( -90, 0, 90 );
			left.transform.Translate( 0, 0.5f, 0 );
			left.name = "left";
		}
	}
	
	void createWinPositions(){}
	
	void mixUpCubes()
	{	
		for( int i = 0 ; i < totalCubes; i++ )
		{
			Transform cube1 = cubes[i].transform;
			Transform randCube2 = cube1;
			while( randCube2 == cube1 )
			{
				int index = Random.Range(0, totalCubes);
				randCube2 = cubes[index].transform;
			}	
			Vector3 cube1Pos = cube1.localPosition;
			cube1.localPosition = randCube2.localPosition;
			randCube2.localPosition = cube1Pos;
				
			cube1.Rotate( new Vector3( 0, 90 * (i%2), 0 ));
		}
	}
	
	void createReflections()
	{
		reflections = new GameObject[totalCubes];
		for( int i = 0; i < totalCubes; i++ ){
			GameObject cube = cubes[i];
			
			reflections[i] = Instantiate( cube, Vector3.zero, Quaternion.identity ) as GameObject;
			reflections[i].transform.parent = reflection;
			
			Vector3 pos = cube.transform.localPosition;
			pos.y *= -1;
			reflections[i].transform.localPosition = pos;
			
			reflections[i].transform.rotation = cube.transform.rotation;
			
			Vector3 scale = cube.transform.localScale;
			scale.y *= -1;
			reflections[i].transform.localScale = scale;
			
			Destroy(reflections[i].GetComponent<Collider>());
			
			cube.GetComponent<CubeData>().reflection = reflections[i].transform;
		}
		//Move floor into place
		float offset = Mathf.Sqrt(maxCubes) - 3;
		Vector3 position = floor.transform.position;
		position.y -= offset/2.0f;
		floor.transform.position = position;
		//Move reflection into place
		position = reflection.transform.position;
		position.y -= offset;
		reflection.transform.position = position;
	}
	
	public readonly static float[] base4PosArr = {-0.5f, 0.5f};
	public readonly static float[] base9PosArr = {-1, 0, 1};
	public readonly static float[] base16PosArr = {-1.5f, -0.5f, 0.5f, 1.5f};
	void setPositionsBase( float[] positionArr )
	{
		//Iterate through each row
		for( int r = 0; r < puzzleHeight; r++ )
		{
			float[] usedXVals = new float[cubesPerRow];
			float[] usedZVals = new float[cubesPerRow];
			
			float yVal = positionArr[r];
			
			for( int i = 0; i < cubesPerRow; i++ ){
				usedXVals[i] = -9999;
				usedZVals[i] = -9999;
			}
			//Iterate through cubes in each row
			for( int c = 0; c < cubesPerRow; c++ )
			{
				int xIndex = 0;
				float randX = -9999;
				while( isInArray( randX, usedXVals, cubesPerRow ))
				{
					xIndex = Random.Range(0, cubesPerRow);
					randX = positionArr[Random.Range(0, cubesPerRow)];
				}
					usedXVals[c] = randX;
				
				int zIndex = 0;
				float randZ = -9999;
				while( isInArray( randZ, usedZVals, cubesPerRow ))
				{
					zIndex = Random.Range(0, cubesPerRow);
					randZ = positionArr[Random.Range(0, cubesPerRow)];
				}
				usedZVals[c] = randZ;
				
				cubes[c + r*cubesPerRow].transform.localPosition = new Vector3( randX, yVal, randZ );
				CubeData cubeData = cubes[c + r*cubesPerRow].GetComponent<CubeData>();
				cubeData.row = r;
				cubeData.xPos = randX;
				cubeData.zPos = randZ;
			}
		}
	}
	
	bool isInArray( float entry, float[] array, int arraySize )
	{
		for( int i = 0; i < arraySize; i++ )
			if( entry == array[i] ) return true;
		
		return false;
	}
}