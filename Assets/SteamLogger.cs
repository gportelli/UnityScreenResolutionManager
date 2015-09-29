using UnityEngine;
using System.Collections.Generic;

public class SteamLogger : MonoBehaviour {
//#if !UNITY_EDITOR
	static Queue<string> queue = new Queue<string>(20);
	void OnEnable() {
		Application.RegisterLogCallback(HandleLog);
	}

	void OnDisable() {
		Application.RegisterLogCallback(null);
	}

	void OnGUI() {
		GUILayout.BeginArea(new Rect(0, Screen.height - 440, Screen.width, 440));
		foreach (string s in queue) {
			GUILayout.Label(s);
		}
		GUILayout.EndArea();
	}

	void HandleLog(string message, string stackTrace, LogType type) {
		queue.Enqueue(Time.time + " - " + message);
		if (queue.Count > 19) {
			queue.Dequeue();
		}
	}
//#endif
}
