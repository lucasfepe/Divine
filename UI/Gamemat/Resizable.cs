using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Resizable : MonoBehaviour
{
    public static Resizable Instance { get; private set; }
    [SerializeField] private RectTransform playerHandBackroundRectTransform;
    [SerializeField] private HorizontalLayoutGroup playerHandHorizontalLayoutGroup;
    private Vector3 lastMousePosition;
    private float widthBeforeResize;
    private RectTransform mainCanvasRectTransform;
    private float minSpacing_5 = -50;
    private float maxSpacing_5 = -273.5f;
    private const float CARD_SPACING_FACTOR = 160f;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        mainCanvasRectTransform = GameObject.Find("MainCanvas").GetComponent<RectTransform>();
        
        
    }

    public void MaxSize()
    {
        int cardsOnHand = PlayerHand.Instance.cardsOnHand;

        float maxSize = 180 + cardsOnHand * CARD_SPACING_FACTOR;
        playerHandBackroundRectTransform.sizeDelta = new Vector2(maxSize, playerHandBackroundRectTransform.sizeDelta.y);

        playerHandHorizontalLayoutGroup.spacing = minSpacing_5;
    }

  

    void OnMouseDown()
    {
        
        widthBeforeResize = playerHandBackroundRectTransform.sizeDelta.x;
        lastMousePosition = Input.mousePosition;
    }

    void OnMouseDrag()
    {
        float distanceX = Input.mousePosition.x - lastMousePosition.x;
        int cardsOnHand = PlayerHand.Instance.cardsOnHand;
        float newSize = ((-1 * distanceX) / Screen.width * mainCanvasRectTransform.rect.width) + widthBeforeResize;
        float maxSize = 180 + cardsOnHand * CARD_SPACING_FACTOR;
        float minSize = .35f  * Screen.width ;
        if (newSize >= maxSize)
        {
            playerHandBackroundRectTransform.sizeDelta = new Vector2(maxSize, playerHandBackroundRectTransform.sizeDelta.y);
            //if (cardsOnHand == 5)
            //{
                playerHandHorizontalLayoutGroup.spacing = minSpacing_5;
            //}
        }
        else if(newSize <= minSize)
        {
            playerHandBackroundRectTransform.sizeDelta = new Vector2(minSize, playerHandBackroundRectTransform.sizeDelta.y);
            //if (cardsOnHand == 5)
            //{
                playerHandHorizontalLayoutGroup.spacing = (-1 * (maxSpacing_5 - minSpacing_5)) * (minSize - maxSize) / maxSize + minSpacing_5;
            //}
        }
        else{
            playerHandBackroundRectTransform.sizeDelta = new Vector2(newSize, playerHandBackroundRectTransform.sizeDelta.y);
            //if(cardsOnHand == 5)
            //{
                
                playerHandHorizontalLayoutGroup.spacing = (-1 * (maxSpacing_5 - minSpacing_5)) * (newSize - maxSize) / maxSize + minSpacing_5;
            //}
        }
        
    }
    public void UpdateSpacing()
    {
        float maxSize = 180 + PlayerHand.Instance.cardsOnHand * CARD_SPACING_FACTOR;
        playerHandHorizontalLayoutGroup.spacing = (-1 * (maxSpacing_5 - minSpacing_5)) * (playerHandBackroundRectTransform.sizeDelta.x - maxSize) / maxSize + minSpacing_5;
    }
    private void OnMouseUp()
    {
    }
}