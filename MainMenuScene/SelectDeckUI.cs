using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectDeckUI : MonoBehaviour
{
    public static SelectDeckUI Instance { get; private set; }

    [SerializeField] private GoButton goButton;
    [SerializeField] private PlayRankedButton playRankedButton;
    [SerializeField] private SelectDecksAreaContent selectDecksUI;
    [SerializeField] private SetButton setNewActiveDeckButton;
    [SerializeField] private SelectDeckBackButton backDeckButton;
    private List<Image> images;
    private List<TextMeshProUGUI> texts;
    private bool isHidden;
    private string activeDeck;
    private List<Button> individualDeckButtons = new List<Button>();
    private Button setNewActiveDeckButtonButton;
    private TextMeshProUGUI setButtonText;

    private IndividualDeckButtonMM selectedButton;
    private void Start()
    {
        playRankedButton.OnPlayRankedButtonPressed += PlayRankedButton_OnPlayRankedButtonPressed;
        selectDecksUI.OnCreateDeckUI += SelectDecksUI_OnCreateDeckUI;
        setNewActiveDeckButton.OnSetNewActiveDeckButtonPressed += SetNewActiveDeckButton_OnSetNewActiveDeckButtonPressed;
        IndividualDeckButtonMM.OnCreateDeckUIButton += IndividualDeckButton_OnCreateDeckUIButton;
        setNewActiveDeckButtonButton = setNewActiveDeckButton.GetComponent<Button>();
        setNewActiveDeckButtonButton.interactable = false;
        setButtonText = setNewActiveDeckButtonButton.GetComponentInChildren<TextMeshProUGUI>();
        setButtonText.alpha = .5f;
        backDeckButton.OnSelectDeckBackButtonPressed += BackDeckButton_OnSelectDeckBackButtonPressed;
        Hide();
    }

    private void BackDeckButton_OnSelectDeckBackButtonPressed(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void IndividualDeckButton_OnCreateDeckUIButton(object sender, System.EventArgs e)
    {
        IndividualDeckButtonMM button = sender as IndividualDeckButtonMM;
        individualDeckButtons.Add(button.GetComponent<Button>());
        button.GetComponent<IndividualDeckButtonMM>().OnSelectDeck += SelectDeckUI_OnSelectDeck;   
    }

    private void SelectDeckUI_OnSelectDeck(object sender, IndividualDeckButtonMM.OnSelectDeckEventArgs e)
    {
        selectedButton = sender as IndividualDeckButtonMM;
        if (!setNewActiveDeckButtonButton.interactable) {
            setButtonText.alpha = 1f;
            setNewActiveDeckButtonButton.interactable = true;
        }
            
    }

    private async void SetNewActiveDeckButton_OnSetNewActiveDeckButtonPressed(object sender, System.EventArgs e)
    {

        

        await LambdaManager.Instance.UpdateActiveDeckLambda(selectedButton.GetDeckTitle());

        individualDeckButtons
            .ToList()
            .ForEach(x => x.GetComponent<Image>().color = Color.white);

        if (selectedButton != null)
        {
            selectedButton
                .GetComponent<Image>()
                .color = Color.green
                ;
        }

        goButton.EnableButton();


    }

    
    private void Awake()
    {
        Instance = this;
        images = GetComponentsInChildren<Image>().ToList(); 
        texts = GetComponentsInChildren<TextMeshProUGUI>().ToList();
        
    }

    private void PlayRankedButton_OnPlayRankedButtonPressed(object sender, System.EventArgs e)
    {
        Show();
    }
    public void SetActiveDeck(string activeDeck)
    {
        this.activeDeck = activeDeck;
    }
    private void SelectDecksUI_OnCreateDeckUI(object sender, SelectDecksAreaContent.OnCreateDeckUIEventArgs e)
    {
        Image image = e.newUI.GetComponent<Image>();
        if (e.newUI.GetComponent<IndividualDeckButtonMM>().GetDeckTitle() == activeDeck) { 
            image.color = Color.green;
            GameObject deckUI = e.newUI as GameObject;
            deckUI.transform.SetSiblingIndex(0);
            //image.material.SetFloat("_ColorChangeTolerance", 0);
        }
        
        List<TextMeshProUGUI> texts = e.newUI.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        images.Add(image);
        this.texts.AddRange(texts);
        if (IsCurrentlyHidden())
        {
            image.enabled = false;
            texts.ForEach(x => x.enabled = false);
        }
    }
    private void Show()
    {
        isHidden = false;
        images.ForEach(x => x.enabled = true);
        texts.ForEach(x => x.enabled = true);
    }
    private void Hide()
    {
        isHidden = true;
        images.ForEach(x => x.enabled = false);
        texts.ForEach(x => x.enabled = false);
    }

    public bool IsCurrentlyHidden()
    {
        return isHidden;
    }
    private void OnDestroy()
    {
        playRankedButton.OnPlayRankedButtonPressed -= PlayRankedButton_OnPlayRankedButtonPressed;
        selectDecksUI.OnCreateDeckUI -= SelectDecksUI_OnCreateDeckUI;
        setNewActiveDeckButton.OnSetNewActiveDeckButtonPressed -= SetNewActiveDeckButton_OnSetNewActiveDeckButtonPressed;
        IndividualDeckButtonMM.OnCreateDeckUIButton -= IndividualDeckButton_OnCreateDeckUIButton;
   
        backDeckButton.OnSelectDeckBackButtonPressed -= BackDeckButton_OnSelectDeckBackButtonPressed;
    }
}
