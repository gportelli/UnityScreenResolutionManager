using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectUtility : MonoBehaviour
{
	static Camera backgroundCam;
	static Camera staticCam; // This is the last camera where Awake is called. It is used for the static getter methods.
	Camera cam;

    void Awake()
    {
        cam = camera;

        if (!cam)
        {
            cam = Camera.main;
        }
        if (!cam)
        {
            Debug.LogError("No camera available");
            return;
        }

		staticCam = cam;

		UpdateCamera ();
    }

    private void UpdateCamera()
    {
        if (!ResolutionManager.FixedAspectRatio || !cam) return;

        float currentAspectRatio = (float)Screen.width / Screen.height;

        // If the current aspect ratio is already approximately equal to the desired aspect ratio,
        // use a full-screen Rect (in case it was set to something else previously)
        if ((int)(currentAspectRatio * 100) / 100.0f == (int)(ResolutionManager.TargetAspectRatio * 100) / 100.0f)
        {
            cam.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            if (backgroundCam)
            {
                Destroy(backgroundCam.gameObject);
            }
            return;
        }

        // Pillarbox
        if (currentAspectRatio > ResolutionManager.TargetAspectRatio)
        {
            float inset = 1.0f - ResolutionManager.TargetAspectRatio / currentAspectRatio;
            cam.rect = new Rect(inset / 2, 0.0f, 1.0f - inset, 1.0f);
        }
        // Letterbox
        else
        {
            float inset = 1.0f - currentAspectRatio / ResolutionManager.TargetAspectRatio;
            cam.rect = new Rect(0.0f, inset / 2, 1.0f, 1.0f - inset);
        }

        if (!backgroundCam)
        {
            // Make a new camera behind the normal camera which displays black; otherwise the unused space is undefined
            backgroundCam = new GameObject("BackgroundCam", typeof(Camera)).camera;
            backgroundCam.depth = int.MinValue;
            backgroundCam.clearFlags = CameraClearFlags.SolidColor;
            backgroundCam.backgroundColor = Color.black;
            backgroundCam.cullingMask = 0;
        }
    }

    public static int screenHeight
    {
        get
        {
			return (int)(Screen.height * staticCam.rect.height);
        }
    }

    public static int screenWidth
    {
        get
        {
			return (int)(Screen.width * staticCam.rect.width);
        }
    }

    public static int xOffset
    {
        get
        {
			return (int)(Screen.width * staticCam.rect.x);
        }
    }

    public static int yOffset
    {
        get
        {
			return (int)(Screen.height * staticCam.rect.y);
        }
    }

    public static Rect screenRect
    {
        get
        {
			return new Rect(staticCam.rect.x * Screen.width, staticCam.rect.y * Screen.height, staticCam.rect.width * Screen.width, staticCam.rect.height * Screen.height);
        }
    }

    public static Vector3 mousePosition
    {
        get
        {
            Vector3 mousePos = Input.mousePosition;
			mousePos.y -= (int)(staticCam.rect.y * Screen.height);
			mousePos.x -= (int)(staticCam.rect.x * Screen.width);
            return mousePos;
        }
    }

    public static Vector2 guiMousePosition
    {
        get
        {
            Vector2 mousePos = Event.current.mousePosition;
			mousePos.y = Mathf.Clamp(mousePos.y, staticCam.rect.y * Screen.height, staticCam.rect.y * Screen.height + staticCam.rect.height * Screen.height);
			mousePos.x = Mathf.Clamp(mousePos.x, staticCam.rect.x * Screen.width, staticCam.rect.x * Screen.width + staticCam.rect.width * Screen.width);
            return mousePos;
        }
    }

	private int lastWidth = -1, lastHeight = -1;
	public void Update()
	{
		if (Screen.width != lastWidth || Screen.height != lastHeight) {
			lastWidth = Screen.width;
			lastHeight = Screen.height;

			UpdateCamera();
		}
	}
}