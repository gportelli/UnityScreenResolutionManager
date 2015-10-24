using UnityEngine;
using System.Collections.Generic;

public class ScreenLogger : MonoBehaviour {
    [Tooltip("Height of the log area as a percentage of the screen height")]
    public float Height = 0.5f;

//#if !UNITY_EDITOR
	static Queue<string> queue = new Queue<string>();
    const int textHeight = 24;


	void OnEnable() {
		Application.logMessageReceived += HandleLog;
	}

	void OnDisable() {
		Application.logMessageReceived -= HandleLog;
	}

	void Update() {
		while (queue.Count > Screen.height * Height / textHeight)
			queue.Dequeue();
	}

	void OnGUI() {
        GUILayout.BeginArea(new Rect(20, Screen.height * (1 - Height) - 10, Screen.width - 20, Screen.height * Height));
		
        foreach (string s in queue) 
			GUILayout.Label(s);
		
		GUILayout.EndArea();
	}

	void HandleLog(string message, string stackTrace, LogType type) {
		queue.Enqueue(message);
	}
//#endif
}
