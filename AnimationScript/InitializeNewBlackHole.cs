using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeNewBlackHole : MonoBehaviour
{
    

    private void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material = new Material(spriteRenderer.material);

        Texture texture = transform.parent.GetComponentInChildren<BlackHoleCamera>().GetTexture();

        spriteRenderer.material.SetTexture("_sceneTexture",texture);

    }
}
