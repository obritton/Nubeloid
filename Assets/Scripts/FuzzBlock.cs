using UnityEngine;
using System.Collections;

public class FuzzBlock : MonoBehaviour {
	
	public Material material;
	
	public void addStatic( int totalStaticScreens )
	{
		for( int i = 0; i < totalStaticScreens; i++ )
		{
			string mainFaceName = "frontFace";
			string oppositeFaceName = "back";
			
			if( i % 2 == 0 )
			{
				mainFaceName = "rightFace";
				oppositeFaceName = "left";
			}
			
			GameObject[] fronts = GameObject.FindGameObjectsWithTag(mainFaceName);
			int fuzzIndex = Random.Range(0, fronts.Length);
			GameObject fuzzFront = fronts[fuzzIndex];
			fuzzFront.GetComponent<Renderer>().material = material;
			GameObject fuzzBack = fuzzFront.transform.parent.FindChild(oppositeFaceName).gameObject;
			fuzzBack.GetComponent<Renderer>().material = material;
			
			fuzzFront.tag = "fuzzed";
			fuzzBack.tag = "fuzzed";
		}
		
		int scale = GetComponent<PuzzleFactory>().cubesPerRow;
		material.SetTextureScale("_DecalTex", new Vector2( scale, scale ));
	}
	
	public void Update()
	{
		material.SetTextureOffset("_MainTex", new Vector2(Random.value, Random.value));
		
	}
}
