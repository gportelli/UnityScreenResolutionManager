﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectUtility : MonoBehaviour
{
    static List<Camera> cams;
    static Camera backgroundCam;

    void Awake()
    {
        if (cams == null) cams = new List<Camera>();

        Camera cam = camera;
        if (!cam)
        {
            cam = Camera.main;
        }
        if (!cam)
        {
            Debug.LogError("No camera available");
            return;
        }

        if (cams.IndexOf(cam) == -1)
            cams.Add(cam);

        SetCamera(cam);
    }

    public static void SetCameras()
    {
        foreach (Camera c in cams)
            SetCamera(c);
    }

    public static void SetCamera(Camera cam)
    {
        if (!ResolutionManager.FixedAspectRatio) return;

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
            return (int)(Screen.height * cams[0].rect.height);
        }
    }

    public static int screenWidth
    {
        get
        {
            return (int)(Screen.width * cams[0].rect.width);
        }
    }

    public static int xOffset
    {
        get
        {
            return (int)(Screen.width * cams[0].rect.x);
        }
    }

    public static int yOffset
    {
        get
        {
            return (int)(Screen.height * cams[0].rect.y);
        }
    }

    public static Rect screenRect
    {
        get
        {
            return new Rect(cams[0].rect.x * Screen.width, cams[0].rect.y * Screen.height, cams[0].rect.width * Screen.width, cams[0].rect.height * Screen.height);
        }
    }

    public static Vector3 mousePosition
    {
        get
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.y -= (int)(cams[0].rect.y * Screen.height);
            mousePos.x -= (int)(cams[0].rect.x * Screen.width);
            return mousePos;
        }
    }

    public static Vector2 guiMousePosition
    {
        get
        {
            Vector2 mousePos = Event.current.mousePosition;
            mousePos.y = Mathf.Clamp(mousePos.y, cams[0].rect.y * Screen.height, cams[0].rect.y * Screen.height + cams[0].rect.height * Screen.height);
            mousePos.x = Mathf.Clamp(mousePos.x, cams[0].rect.x * Screen.width, cams[0].rect.x * Screen.width + cams[0].rect.width * Screen.width);
            return mousePos;
        }
    }
}