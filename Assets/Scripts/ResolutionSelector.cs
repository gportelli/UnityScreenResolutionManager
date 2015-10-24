using UnityEngine;
using System.Collections;

public class ResolutionSelector : MonoBehaviour {
    void OnGUI()
    {
        if (ResolutionManager.Instance == null) return;

        ResolutionManager resolutionManager = ResolutionManager.Instance;

        GUILayout.BeginArea(new Rect(20, 10, 200, Screen.height - 10));

        GUILayout.Label("Select Resolution");

        if (GUILayout.Button(Screen.fullScreen ? "Windowed" : "Fullscreen"))
            resolutionManager.ToggleFullscreen();

        int i = 0;
        foreach (Vector2 r in Screen.fullScreen ? resolutionManager.FullscreenResolutions : resolutionManager.WindowedResolutions)
        {
            string label = r.x + "x" + r.y;
            if (r.x == Screen.width && r.y == Screen.height) label += "*";
            if (r.x == resolutionManager.DisplayResolution.width && r.y == resolutionManager.DisplayResolution.height) label += " (native)";

            if (GUILayout.Button(label))
                resolutionManager.SetResolution(i, Screen.fullScreen);

            i++;
        }

        GUILayout.EndArea();
    }
}
