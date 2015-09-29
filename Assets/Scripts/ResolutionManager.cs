using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ResolutionManager : MonoBehaviour {
    static public ResolutionManager Instance;

    // Fixed aspect ratio parameters
    static public bool  FixedAspectRatio = true;
    static public float TargetAspectRatio = 16f / 9f;

    // Windowed aspect ratio when FixedAspectRatio is false
    static public float WindowedAspectRatio = 4f / 3f;

    // List of horizontal resolution to include
    int[] resolutions = new int[] { 600, 800, 1024, 1280, 1400, 1600, 1920 };

    public Resolution NativeResolution;
    public List<Vector2> WindowedResolutions, FullscreenResolutions;

    int currWindowedRes, currFullscreenRes;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(StartRoutine());
    }

	private void printResolution()
	{
		Debug.Log ("Current res: " + Screen.currentResolution.width + "x" + Screen.currentResolution.height);
	}

    private IEnumerator StartRoutine()
    {
		if (Application.platform == RuntimePlatform.OSXPlayer) {
			NativeResolution = Screen.currentResolution;
		}
		else 
		{
			if (Screen.fullScreen) {
				Resolution r = Screen.currentResolution;
				Screen.fullScreen = false;

				yield return null;
				yield return null;

				NativeResolution = Screen.currentResolution;

				Screen.SetResolution (r.width, r.height, true);

				yield return null;
			} else {
				NativeResolution = Screen.currentResolution;
			}
		}

        InitResolutions();
    }

    private void InitResolutions()
    {
        float screenAspect = (float)NativeResolution.width / NativeResolution.height;

        WindowedResolutions   = new List<Vector2>();
        FullscreenResolutions = new List<Vector2>();

        foreach(int w in resolutions) {
            if (w < NativeResolution.width)
            {
                // Adding resolution only if it's 20% smaller than the screen
                if (w < NativeResolution.width * 0.8f)
                {
                    Vector2 windowedResolution = new Vector2(w, Mathf.Round(w / (FixedAspectRatio ? TargetAspectRatio : WindowedAspectRatio)));
                    if (windowedResolution.y < NativeResolution.height * 0.8f)
                        WindowedResolutions.Add(windowedResolution);

                    FullscreenResolutions.Add(new Vector2(w, Mathf.Round(w / screenAspect)));
                }
            }
        }

        // Adding fullscreen native resolution
        FullscreenResolutions.Add(new Vector2(NativeResolution.width, NativeResolution.height));

        // Adding half fullscreen native resolution
        Vector2 halfNative = new Vector2(NativeResolution.width * 0.5f, NativeResolution.height * 0.5f);
        if (halfNative.x > resolutions[0] && FullscreenResolutions.IndexOf(halfNative) == -1)
            FullscreenResolutions.Add(halfNative);

        FullscreenResolutions = FullscreenResolutions.OrderBy(resolution => resolution.x).ToList();

        bool found = false;

        if (Screen.fullScreen)
        {
            currWindowedRes = WindowedResolutions.Count - 1;

            for (int i = 0; i < FullscreenResolutions.Count; i++)
            {
                if(FullscreenResolutions[i].x == Screen.width && FullscreenResolutions[i].y == Screen.height) {
                    currFullscreenRes = i;
                    found = true;
                    break;
                }
            }

            if (!found)
                SetResolution(FullscreenResolutions.Count - 1, true);
        }
        else
        {
            currFullscreenRes = FullscreenResolutions.Count - 1;

            for (int i = 0; i < WindowedResolutions.Count; i++)
            {
                if (WindowedResolutions[i].x == Screen.width && WindowedResolutions[i].y == Screen.height)
                {
                    found = true;
                    currWindowedRes = i;
                    break;
                }
            }

            if (!found)
                SetResolution(WindowedResolutions.Count - 1, false);
        }
    }

    public void SetResolution(int index, bool fullscreen)
    {
        Vector2 r = new Vector2();
        if (fullscreen)
        {
            currFullscreenRes = index;
            r = FullscreenResolutions[currFullscreenRes];
        }
        else
        {
            currWindowedRes = index;
            r = WindowedResolutions[currWindowedRes];
        }

		bool fullscreen2windowed = Screen.fullScreen & !fullscreen;

		Screen.SetResolution ((int)r.x, (int)r.y, fullscreen);

		if (Application.platform == RuntimePlatform.OSXPlayer) {
			StopAllCoroutines();

			if(fullscreen2windowed) StartCoroutine (SetResolutionAfterResize (r));
		}
    }

	private IEnumerator SetResolutionAfterResize(Vector2 r) {
		int maxTime = 5;
		float time = Time.time;

		yield return null;
		yield return null;

		int lastW = Screen.width;
		int lastH = Screen.height;

		while (Time.time - time < maxTime) {
			if(lastW != Screen.width || lastH != Screen.height) {
				Debug.Log ("Resize! " + Screen.width + "x" + Screen.height);

				Screen.SetResolution ((int)r.x, (int)r.y, Screen.fullScreen);
				yield break;

				lastW = Screen.width; lastH = Screen.height;
			}

			yield return null;
		}

		Debug.Log ("End waiting");
	}

    public void ToggleFullscreen()
    {
        SetResolution(
            Screen.fullScreen ? currWindowedRes : currFullscreenRes,
            !Screen.fullScreen);
    }
}
