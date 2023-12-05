using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleCamera : MonoBehaviour
{
    private new Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        camera.targetTexture = new RenderTexture(camera.targetTexture);
    }

    public Texture GetTexture() { return camera.targetTexture; }
}
