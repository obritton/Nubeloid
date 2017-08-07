using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {
	
	static int currentSong = -1;
	
	void Start () {
		DontDestroyOnLoad( gameObject );
		Application.LoadLevel("1_MainMenu");
	}
}