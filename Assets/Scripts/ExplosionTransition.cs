using UnityEngine;
using System.Collections;

public class ExplosionTransition : MonoBehaviour {
	
	public GameObject cubeloid;
	public static bool isExploding = false;
	public GameObject floor;
	
	public IEnumerator doExplosionTransition()
	{
		floor.GetComponent<Collider>().enabled = true;
		explodeCubes();
		moveOff();
		yield return new WaitForSeconds(0.5f);
		transitionToNewScene();
	}
	
	public void explodeCubes()
	{
		isExploding = true;
		
		for( int i = 0; i < cubeloid.transform.GetChildCount(); i++ )
		{
			GameObject cube = cubeloid.transform.GetChild(i).gameObject;
			cube.AddComponent<Rigidbody>();
			Vector3 direction = cube.transform.position * 20;
			direction.y *= 5;
			if( cube.transform.localPosition.y < 0 )
				direction.y *= 2.5f;
			cube.GetComponent<Rigidbody>().AddForce(direction);
			cube.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-1000.0F, 1000.0F), Random.Range(-1000.0F, 1000.0F), Random.Range(-1000.0F, 1000.0F)));
			cube.GetComponent<Collider>().isTrigger = false;
		}
	}
	
	public void stopExploding()
	{
		for( int i = 0; i < cubeloid.transform.GetChildCount(); i++ )
		{
			GameObject cube = cubeloid.transform.GetChild(i).gameObject;
			cube.GetComponent<Rigidbody>().velocity = Vector3.zero;
			cube.GetComponent<Rigidbody>().isKinematic = true;
		}
	}
	
	void moveOff()
	{
		Vector3 cubeloidTargetPos = cubeloid.transform.position;
		cubeloidTargetPos.x -= 7;
		
		iTween.MoveTo( cubeloid, cubeloidTargetPos, 1.5f );
	}
	
	void transitionToNewScene()
	{
		EndlessLevelsHandler.buildRandomLevel();
		Application.LoadLevel("6_EndlessGamePlay");
	}
	
	void Update()
	{
		if( isExploding )
		{
			for( int i = 0; i < cubeloid.transform.GetChildCount(); i++ )
			{
				GameObject cube = cubeloid.transform.GetChild(i).gameObject;
				Transform reflection = cube.GetComponent<CubeData>().reflection;
				Quaternion rot = cube.transform.rotation;
				rot.eulerAngles = Vector3.Reflect(rot.eulerAngles, Vector3.forward);
				rot.eulerAngles = Vector3.Reflect(rot.eulerAngles, Vector3.right);
				reflection.rotation = rot;
				Vector3 pos = cube.transform.position;
				pos.y *= -1;
				reflection.position = pos;
			}
		}
	}
}
