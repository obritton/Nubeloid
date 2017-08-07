using UnityEngine;
using System.Collections;

public class CubeloidFactory : MonoBehaviour {
	
	public bool mixEm = true;//
	public static int totalCubes = 16;//
	public static int imageNumber = 1;
	public GameObject cubePrefab;//
	public Transform cubeloid;//
	public Transform reflection;//
	private GameObject[] cubes;//
	private GameObject[] reflections;//
	public Transform floor;
	
	public Material materialFront;//
	public Material materialSide;//
	
	public GameObject frontPreview;
	public GameObject sidePreview;
	
	public static GameObject[] orderedCubesFront;
	public static GameObject[] orderedCubesSide;
	
	void Awake (){
		
		createCubes();
		applyTexturedFaces();
		if( mixEm )
			mixUpCubes();
		
		createReflections();
	}
	
	void mixUpCubes()
	{
		for( int i = 0 ; i < totalCubes; i++ )
		{
			Transform cube1 = cubes[i].transform;
			Transform randCube2 = cubes[Random.Range(0, totalCubes)].transform;
				
			Vector3 cube1Pos = cube1.localPosition;
			cube1.localPosition = randCube2.localPosition;
			randCube2.localPosition = cube1Pos;
				
			cube1.Rotate( new Vector3( 0, 90 * (i%2), 0 ));
		}
	}
	
	ArrayList myArrayList = new ArrayList();
	
	void applyTexturedFaces(){
		
		int frontI = Random.Range(1,16);
		int sideI = frontI;
		while( sideI == frontI )
			sideI = Random.Range(1,16);
		
		Texture2D frontTex = (Texture2D)Resources.Load("FruitTextures/" + frontI);
		materialFront.SetTexture( "_MainTex", frontTex );
		Texture2D sideTex = (Texture2D)Resources.Load("FruitTextures/" + sideI);
		materialSide.SetTexture( "_MainTex", sideTex );
		
		GameObject faceBundle;
		switch( totalCubes ){
		case 4:
		default:
			faceBundle = (GameObject)Resources.Load("Quad_Faces");
			break;
		case 9:
			faceBundle = (GameObject)Resources.Load("Nova_Faces");
			break;
		case 16:
			faceBundle = (GameObject)Resources.Load("Hex_Faces");
			break;
		}
		
		GameObject[] faces = faceBundle.GetComponent<FaceIndexer>().faces;
		
		foreach( GameObject cube in cubes ){
			GameObject front, back, left, right ;
			int fIndex = 0;
			int sIndex = 0;
			Vector3 pos = cube.transform.localPosition;
			
			switch( totalCubes ){
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
			
			orderedCubesFront[fIndex] = cube;
			orderedCubesSide[sIndex] = cube;
			
			front = Instantiate(faces[fIndex], cube.transform.position, cube.transform.rotation) as GameObject;
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
			
			myArrayList.Add( left);
			myArrayList.Add(front);
			myArrayList.Add(back);
			myArrayList.Add(right);
		}
	}
	
	void createCubes()
	{
		cubes = new GameObject[totalCubes];
		for( int i = 0; i < totalCubes; i++ ){
			cubes[i] = Instantiate( cubePrefab, Vector3.zero, Quaternion.identity ) as GameObject;
			cubes[i].transform.parent = cubeloid;
		}
		cubeloid.Rotate( 0, -90, 0);
		
		orderedCubesFront = new GameObject[totalCubes];
		orderedCubesSide = new GameObject[totalCubes];
		
		switch( totalCubes ){
		case 4:
			setPositions4();
			break;
		case 9:
			setPositions9();
			break;
		case 16:
			setPositions16();
			break;
		}
		cubeloid.Rotate( 0, 90, 0 );
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
		float offset = Mathf.Sqrt(totalCubes) - 3;
		Vector3 position = floor.transform.position;
		position.y -= offset/2.0f;
		floor.transform.position = position;
		//Move relefction into place
		position = reflection.transform.position;
		position.y -= offset;
		reflection.transform.position = position;
	}
	
	void setPositions4(){
		for( int y = 0; y < 2; y++ ){
			int x1 = Random.value < 0.5 ? -1 : 1;
			int x2 = -x1;
			int z1 = Random.value < 0.5 ? -1 : 1;
			int z2 = - z1;
			int adjustedY = y * 2 - 1;
			
			cubes[y*2].transform.localPosition = new Vector3(x1,adjustedY,z1)/2;
			cubes[y*2+1].transform.localPosition = new Vector3(x2,adjustedY,z2)/2;
		}
	}
	
	void setPositions9(){
		for( int y = -1; y < 2; y++){
			int x1 = Random.Range(-1,2);
			int x2, x3;
			
			do{
				x2 = Random.Range(-1,2);
			}while( x2 == x1 );
			x3 = -(x1 + x2);
			
			int z1 = Random.Range(-1,2);
			int z2, z3;
			do{
				z2 = Random.Range(-1,2);
			}while( z2 == z1 );
			z3 = -(z1 + z2);
			
			cubes[(y+1)*3].transform.localPosition = new Vector3(x1,y,z1);
			cubes[(y+1)*3+1].transform.localPosition = new Vector3(x2,y,z2);
			cubes[(y+1)*3+2].transform.localPosition = new Vector3(x3,y,z3);
		}
	}
	
	void setPositions16(){
		for( int y = 0; y < 4; y++ ){
			int x1 = Random.value < 0.5 ? -1 : 1;
			int x2 = -x1;
			int x3 = x1 * 3;
			int x4 = x2 * 3;
			
			int z1 = Random.value < 0.5 ? -1 : 1;
			int z2 = -z1;
			int z3 = z1 * 3;
			int z4 = z2 * 3;
			
			//Map 0, 1, 2, 3 onto -3, -1, 1, 3
			float adjustedY = (y-1.5f) * 2.0f;
			
			cubes[y*4].transform.localPosition = new Vector3(x1,adjustedY,z1)/2;
			cubes[y*4+1].transform.localPosition = new Vector3(x2,adjustedY,z2)/2;
			cubes[y*4+2].transform.localPosition = new Vector3(x3,adjustedY,z3)/2;
			cubes[y*4+3].transform.localPosition = new Vector3(x4,adjustedY,z4)/2;
		}
	}
	
	void setupCameraPosition(){
		//Map total cubes 4, 9, 16 onto camera distances -8, -12, -16
		Vector3 camPos = Camera.main.transform.position;
		camPos.z = Mathf.Sqrt(totalCubes) * - 3 + 1.0f;
		Camera.main.transform.position = camPos;
	}
}
