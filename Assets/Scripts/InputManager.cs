using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	
	public GameObject cubeloid;
	public GameObject reflection;
	public GameObject camPivot;
	
	private GameObject liveCube;
	private Vector2 touchStart;
	private bool swipeProcessed;
	private float edgeDistance;
	private bool isWon = false;
	private bool moved = false;
	private int buttonWidth;
	public GUIStyle buttonStyle;
	private bool flattened = false;
	
	private int puzzleHeight;
	private float puzzleBasement;
	private int totalPuzzles = 0;
	private int currentPuzzle = 0;
	
	Rect flattenBtnRec = new Rect( 0, 0, 70, 70);
	
	void Start (){
		//Map maxCubes 2, 3, 4 onto distances 0.5, 1.0, 1.5
		int cubesPerRow = GetComponent<PuzzleFactory>().cubesPerRow;
		puzzleHeight = GetComponent<PuzzleFactory>().puzzleHeight;
		edgeDistance = 0.5f * (cubesPerRow - 1.0f) + 0.1f;
		buttonWidth = Screen.height/10;
		ExplosionTransition.isExploding = false;
		
		totalPuzzles = PuzzleFactory.levelConfig.totalPuzzles;
		currentPuzzle = PuzzleFactory.levelConfig.currentPuzzle;
		
		if( cubesPerRow == 3 )
			puzzleBasement = -2;
		else
			puzzleBasement = cubesPerRow == 2 ? -1 : -2;
	}
	
	float offset = 1;
	public GUIStyle collapseBtnStl;
	public GUIStyle titleBtnStl;
	
	void OnGUI()
	{
		if( isWon )
		{
			if( GUI.Button( new Rect(-offset, 0, Screen.width, Screen.height + 5), "NEXT", buttonStyle ))
			{	
				if( !ExplosionTransition.isExploding )
				{
					ExplosionTransition explodeScript = (ExplosionTransition)GetComponent<ExplosionTransition>();
					StartCoroutine( explodeScript.doExplosionTransition());
				}
			}
		}
		else
		{
			GUI.Label( new Rect( 10, Screen.height - 60, 50, 50 ), "", collapseBtnStl );
			GUI.Label ( new Rect( 62, Screen.height - 68, 256, 64) , "", titleBtnStl );
		}
		
		if( ExplosionTransition.isExploding )
			offset*=1.1f;
	}
	
//----------------------------------------------------------- INPUT ----------
	void Update (){
		if( !isWon )
		{
			//Touch the flatten button area
			if( !flattened )	
			{
				if( flattenBtnRec.Contains( Input.mousePosition ) && Input.GetMouseButtonDown(0))
				{
					flattened = true;
					doCollapse();
				}
				else
				{
					if( Input.GetMouseButtonDown(0)){
						touchStart = Input.mousePosition;
						liveCube = raycastForCube( touchStart );
						swipeProcessed = false;
						moved = false;
					}
					else if( Input.GetMouseButton(0))
					{
						Vector2 touchLoc = (Vector2)Input.mousePosition;
						Vector2 touchDelta = (touchStart - touchLoc);
						if( touchDelta.magnitude > 0 && !swipeProcessed){
							if( liveCube )
								move( touchDelta );
							else
								rotate( touchDelta );
							swipeProcessed = true;
						}
					}
					else if( Input.GetMouseButtonUp(0)){
						if( !moved && !swipeProcessed )
							rotate( Vector2.zero );
						liveCube = null;
					}
				}
			}
			else//if flattened
			{
				if( Input.GetMouseButtonUp(0))
					doUnCollapse();
			}
		}
	}
	
	void doCollapse()
	{
		float rot = cubeloid.transform.eulerAngles.y;
		while( rot < 0 ) rot += 360;
		while( rot >= 360 ) rot -= 360;
		string collapseAxis = "x";
		float flatPos = 1;
		
		if( Mathf.Abs( rot ) < 15 )
		{
			flatPos = -1;
			collapseAxis = "z";
		}
		
		if( cubeloid.tag == "quadraloid" )
			flatPos *= 0.5f;
		else if( cubeloid.tag == "hexaloid" )
			flatPos *= 1.5f;
		
		for( int i = 0; i < cubeloid.transform.childCount; i++ )
		{
			GameObject cube = cubeloid.transform.GetChild(i).gameObject;
			CubeData data = (CubeData)cube.GetComponent<CubeData>();
			data.preCompressPos = collapseAxis == "x" ? cube.transform.localPosition.x : cube.transform.localPosition.z;
			
			iTween.MoveTo( cube, iTween.Hash( collapseAxis, flatPos, "islocal", true, "time", 0.2f ));
			iTween.MoveTo( data.reflection.gameObject, iTween.Hash( collapseAxis, flatPos, "islocal", true, "time", 0.2f ));
		}
	}
	
	void doUnCollapse()
	{
		float rot = cubeloid.transform.eulerAngles.y;
		while( rot < 0 ) rot += 360;
		while( rot >= 360 ) rot -= 360;
		string collapseAxis = Mathf.Abs( rot ) < 15 ? "z" : "x";
		
		for( int i = 0; i < cubeloid.transform.childCount; i++ )
		{
			GameObject cube = cubeloid.transform.GetChild(i).gameObject;
			CubeData data = (CubeData)cube.GetComponent<CubeData>();
			if( i == 0)
				iTween.MoveTo( cube, iTween.Hash( collapseAxis, data.preCompressPos, "time", 0.2f, "islocal", true, "oncompletetarget", gameObject, "onComplete", "uncollapseDidComplete" ));
			else
				iTween.MoveTo( cube, iTween.Hash( collapseAxis, data.preCompressPos, "time", 0.2f, "islocal", true ));
			
			iTween.MoveTo( data.reflection.gameObject, iTween.Hash( collapseAxis, data.preCompressPos, "time", 0.2f, "islocal", true ));
		}
	}
	
	void uncollapseDidComplete()
	{
		flattened = false;
	}
	
	void resetSwipe()
	{
		swipeProcessed = false;
		touchStart = Input.mousePosition;
	}
	
	GameObject raycastForCube( Vector2 pickPoint ){
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if( Physics.Raycast(ray, out hit, 20 ))
			return hit.collider.gameObject;
		
		return null;
	}
//----------------------------------------------------------- LOGIC ----------
	void checkForWin()
	{
		bool win = true;
		
		for( int i = 0; i < puzzleHeight; i++ )
		{
			WinManager.RowState rowComp = GetComponent<WinManager>().getRowState(i);
			if( rowComp != WinManager.RowState.AllCubesRotAndPositioned )
				win = false;
		}
		
		resetSwipe();
		
		if( win )
			processWin();
	}
	
	void processWin()
	{
		TimerManager tm = GetComponent<TimerManager>();
		if( tm != null )
		{
			tm.stopCounter();
			print ("tm.currentTime: " + tm.currentTime);
			PuzzleFactory.levelConfig.currentTimer = tm.currentTime;
		}
		
		if( currentPuzzle < totalPuzzles )
			Application.LoadLevel("4_AnyHeight");
		else
			isWon = true;
	}
//------------------------------------------------------------ ANIM ----------
	void move( Vector2 delta ){
		delta = ( Mathf.Abs( delta.x ) >= Mathf.Abs( delta.y )) ? new Vector2( -Mathf.Sign(delta.x), 0 ) : new Vector2( 0, -Mathf.Sign(delta.y));
		
		Vector3 delta3 = new Vector3(delta.x, delta.y, 0f);
		
		float rot = cubeloid.transform.eulerAngles.y;
		while( rot < 0 ) rot += 360;
		while( rot >= 360 ) rot -= 360;
		if( Mathf.Abs( rot ) > 15 )
		{
			float swap = delta3.z;
			delta3.z = delta3.x;
			delta3.x = swap;
		}
		
		GameObject cubeReflection = liveCube.GetComponent<CubeData>().reflection.gameObject;
		if( canMove( liveCube.transform.localPosition + delta3 ))
		{			
			iTween.MoveBy( liveCube, iTween.Hash( "amount", (Vector3)delta, "time", 0.25, "space", Space.World, "oncomplete", "checkForWin", "oncompletetarget", gameObject, "easeType", iTween.EaseType.easeOutCubic ));
			delta.y *= -1;
			iTween.MoveBy( cubeReflection, iTween.Hash( "amount", (Vector3)delta, "time", 0.25, "space", Space.World, "easeType", iTween.EaseType.easeOutCubic ));
			GetComponent<AudioSource>().Play();
		}
		else{
			iTween.PunchPosition( liveCube, iTween.Hash( "amount", (Vector3)delta*0.1f, "time", 0.25, "space", Space.World, "easeType", iTween.EaseType.easeOutCubic ));
			delta.y *= -1;
			iTween.PunchPosition( cubeReflection, iTween.Hash( "amount", (Vector3)delta*0.1f, "time", 0.25, "space", Space.World, "easeType", iTween.EaseType.easeOutCubic ));
		}
		
		moved = true;
	}
	
	bool canMove( Vector3 ghostPos )
	{
		if( Mathf.Abs(ghostPos.x) > edgeDistance || Mathf.Abs(ghostPos.z) > edgeDistance || Mathf.Abs(ghostPos.y) > edgeDistance || ghostPos.y > puzzleBasement + puzzleHeight + 0.1 )
			return false;

		foreach( Transform cube in cubeloid.transform )
			if( Vector3.Distance( cube.localPosition, ghostPos ) < 0.1 )
				return false;
		return true;
	}
	
	void rotate( Vector2 delta ){
		GetComponent<AudioSource>().Play();
		if( liveCube )
		{
			if( liveCube.name != "anchor" )
			{
				GameObject cubeReflection = liveCube.GetComponent<CubeData>().reflection.gameObject;
				
				iTween.RotateAdd( liveCube, iTween.Hash( "y", 90, "time", 0.4, "space", Space.Self, "oncomplete", "checkForWin", "oncompletetarget", gameObject, "easeType", iTween.EaseType.easeOutBack ));
				iTween.RotateAdd( cubeReflection, iTween.Hash( "y", 90, "time", 0.4, "space", Space.Self, "easeType", iTween.EaseType.easeOutBack ));
			}
		}
		else
		{
			float rot = cubeloid.transform.eulerAngles.y;
			while( rot < 0 ) rot += 360;
			while( rot >= 360 ) rot -= 360;
			float targetRot = Mathf.Abs( rot ) < 15 ? 90 : 0;
			
			if( delta == Vector2.zero )
			{
				iTween.RotateTo( cubeloid, iTween.Hash( "y", targetRot, "time", 0.4, "easeType", iTween.EaseType.easeInOutSine ));
				iTween.RotateTo( reflection, iTween.Hash( "y", targetRot, "time", 0.4, "easeType", iTween.EaseType.easeInOutSine ));
			}
			else
			{
				if( Mathf.Abs(delta.y) > Mathf.Abs(delta.x) )
				{
					iTween.PunchPosition( cubeloid, iTween.Hash( "amount", (Vector3)delta.normalized*-0.1f, "time", 0.4));
					iTween.PunchPosition( reflection, iTween.Hash( "amount", (Vector3)delta.normalized*0.1f, "time", 0.4));
				}
				else
				{
					if( delta.x > 0 )
					{
						if( rot < 15 )
						{
							iTween.RotateTo( cubeloid, iTween.Hash( "y", targetRot, "time", 0.4, "easeType", iTween.EaseType.easeInOutSine ));
							iTween.RotateTo( reflection, iTween.Hash( "y", targetRot, "time", 0.4, "easeType", iTween.EaseType.easeInOutSine ));
						}
						else
						{
							iTween.PunchRotation( cubeloid, iTween.Hash( "y", 10, "time", 0.4, "easeType", iTween.EaseType.punch ));
							iTween.PunchRotation( reflection, iTween.Hash( "y", 10, "time", 0.4, "easeType", iTween.EaseType.punch ));
						}
					}
					else if( delta.x < 0 )
					{
						if( rot > 15 )
						{
							iTween.RotateTo( cubeloid, iTween.Hash( "y", targetRot, "time", 0.4, "easeType", iTween.EaseType.easeInOutSine ));
							iTween.RotateTo( reflection, iTween.Hash( "y", targetRot, "time", 0.4, "easeType", iTween.EaseType.easeInOutSine ));
						}
						else
						{
							iTween.PunchRotation( cubeloid, iTween.Hash( "y", -10, "time", 0.4, "easeType", iTween.EaseType.punch ));
							iTween.PunchRotation( reflection, iTween.Hash( "y", -10, "time", 0.4, "easeType", iTween.EaseType.punch ));
						}
					}
				}
			}
		}
	}
}