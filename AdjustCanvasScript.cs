using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustCanvasScript : MonoBehaviour
{
    private float prevWidth;
    private float prevHeight;
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        prevWidth = Screen.width;
        prevHeight = Screen.height;
    }

    private IEnumerator RefreshCanvasSize()
    {
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        yield return new WaitForEndOfFrame();
        canvas.renderMode = RenderMode.WorldSpace;
    }

    private void Update()
    {
        if(prevWidth != Screen.width || prevHeight != Screen.height)
        {
            StartCoroutine(RefreshCanvasSize());
            prevWidth = Screen.width;
            prevHeight = Screen.height;
        }
    }
}
