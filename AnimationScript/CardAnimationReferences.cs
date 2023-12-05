using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardAnimationReferences : MonoBehaviour
{
    //public static CardAnimationReferences Instance { get; private set; }

    [SerializeField] private Image cardOverlay;
    [SerializeField] private Image cardBorder;
    [SerializeField] private Image skillImage;
    [SerializeField] private Image timeIconImage;
    [SerializeField] private Image shedImage;
    [SerializeField] private Image stardustImage;
    [SerializeField] private Image lightImage;
    [SerializeField] private Image redGiantImage;
    [SerializeField] private Image graphicImage;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI shedText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI stardustText;
    [SerializeField] private TextMeshProUGUI lightText;
    [SerializeField] private Transform statuses;

    private Material newMaterial;

    public Material GetMaterial() { return newMaterial; }
    private void Awake()
    {
        //Instance = this;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        newMaterial = new Material(spriteRenderer.material);
        spriteRenderer.material = newMaterial;
    }

    public Image GetCardOverlay(){return cardOverlay;}
    public Image GetCardBorder(){return cardBorder;}
    public Image GetSkillImage(){return skillImage;}
    public Image GetTimeIconImage(){return timeIconImage;}
    public Image GetShedImage(){return shedImage;}
    public Image GetStardustImage(){return stardustImage;}
    public Image GetLightImage(){return lightImage;}
    public Image GetRedGiantImage(){return redGiantImage;}
    public Image GetGraphicImage(){return graphicImage;}
    public TextMeshProUGUI GetTimeText(){return timeText;}
    public TextMeshProUGUI GetShedText(){return shedText;}
    public TextMeshProUGUI GetTitleText(){return titleText;}
    public TextMeshProUGUI GetStardustText(){return stardustText;}
    public TextMeshProUGUI GetLightText(){return lightText;}
    public Transform GetStatuses(){return statuses;}
}
