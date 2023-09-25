using Mono.CSharp.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpponentHandResponsive : MonoBehaviour
{
    public static OpponentHandResponsive Instance { get; private set; }

    [SerializeField] private RectTransform opponentDeckUI;
    [SerializeField] private RectTransform opponentHand;

    public event EventHandler OnLoad;

    public const float TALL_SCREEN_CARD_SCALE = .435f;
    public const float LONG_SCREEN_CARD_SCALE = .65f;
    private const float KEY_HEIGHT_TO_WIDHT_RATIO = .59f;
    private float height_to_width_ratio = 0;
    private int opponentDeckUIPosX;
    private float opponentDeckUIPosY;
    private int opponentHandTop;
    private int opponentHandBottom;
    private int opponentHandLeft;
    private int opponentHandBackgroundWidth;
    private int opponentHandBackgroundHeight;
    private RectTransform rectTransform;
    private Image opponentHandBackgroundImage;
    private bool hasResponded = false;
    private Sprite spriteImage;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        opponentHandBackgroundImage = GetComponent<Image>();

        height_to_width_ratio = (float)Screen.height / Screen.width;
        if (height_to_width_ratio > KEY_HEIGHT_TO_WIDHT_RATIO)
        {
            opponentDeckUIPosX = 110;
            opponentDeckUIPosY = 72.5f;
            opponentHandTop = 145;
            opponentHandBottom = 0;
            opponentHandLeft = -25;
            opponentHandBackgroundWidth = 220;
            opponentHandBackgroundHeight = 290;
            spriteImage = Image2SpriteUtility.Instance.GetTallTablet();
        }
        else
        {
            opponentDeckUIPosX = 75;
            opponentDeckUIPosY = 0;
            opponentHandTop = 0;
            opponentHandBottom = 0;
            opponentHandLeft = 145;
            opponentHandBackgroundWidth = 415;
            opponentHandBackgroundHeight = 220;
            spriteImage = Image2SpriteUtility.Instance.GetLongTablet();
        }
        Respond();

        //CardGameManager.Instance.On
    }

    public void ResponsiveCard(RectTransform rectTransform)
    {
        if (height_to_width_ratio < KEY_HEIGHT_TO_WIDHT_RATIO)
        {
            //long screen
            rectTransform.localScale = new Vector2(LONG_SCREEN_CARD_SCALE, LONG_SCREEN_CARD_SCALE);
        }
        else
        {
            //tall screen
            rectTransform.localScale = new Vector2(TALL_SCREEN_CARD_SCALE, TALL_SCREEN_CARD_SCALE);
        }
    }

    //need to call this after network bojects is spawned too 
    //because the spawning will reset it
    public void RespondOnce()
    {
        if (!hasResponded)
        { 

            hasResponded = true;
            Respond();
        }
        OnLoad?.Invoke(this, EventArgs.Empty);
    }
    private void Respond()
    {
        opponentDeckUI.anchoredPosition = new Vector2(opponentDeckUIPosX, opponentDeckUIPosY);
        opponentHand.offsetMax = new Vector2(opponentHand.offsetMax.x, -1 * opponentHandTop);
        opponentHand.offsetMin = new Vector2(opponentHandLeft, opponentHandBottom);
        rectTransform.sizeDelta = new Vector2(opponentHandBackgroundWidth, opponentHandBackgroundHeight);
        opponentHandBackgroundImage.sprite = spriteImage;
    }
    private void RespondHand()
    {

    }
}

