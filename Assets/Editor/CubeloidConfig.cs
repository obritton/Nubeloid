using UnityEngine;
using System.Collections;
using UnityEditor;

public class CubeloidConfig : EditorWindow {
	
	static void ShowWindow()
	{
		EditorWindow.GetWindow (typeof(CubeloidConfig));
    }
	
	void OnGUI()
	{
		GUI.Label( new Rect(10, 10, 100, 40), "Puzzle Config" );
	}
}
