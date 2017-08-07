using UnityEngine;
using System.Collections;

public class EntryAnim : MonoBehaviour {
	
	public GameObject cubeloid;
	public GameObject reflection;
	
	void Start ()
	{
		//Sort and stagger cubes bottom up, with a bounce
		cubeloid.transform.Translate( 0, 5, 0 );
		iTween.MoveBy( cubeloid, new Vector3( 0, -5, 0), 1.0f);
		reflection.transform.Translate( 0, -5, 0 );
		iTween.MoveBy( reflection, new Vector3( 0, 5, 0), 1.0f);
	}
}
