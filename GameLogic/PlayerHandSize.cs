using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandSize : MonoBehaviour
{
    private RectTransform rectTransform;
    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    //private void Update()
    //{
    //    int cardOnHand = PlayerHand.Instance.cardsOnHand;
    //    rectTransform.sizeDelta = new Vector2(180 + cardOnHand * 150, 220);
    //    //boxCollider2D.size = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
    //    //boxCollider2D.offset = new Vector2(-1 * rectTransform.sizeDelta.x / 2, rectTransform.sizeDelta.y / 2);
    //}


}
