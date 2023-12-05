using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsMouseInsideRectangle : MonoBehaviour
{

    private RectTransform rectTransform;
    private Camera camera;
    private void Awake()
    {
        camera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
    }

    public bool IsMouseInside()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, camera);
    }
   
}
