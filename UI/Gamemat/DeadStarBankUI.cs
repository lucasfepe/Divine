using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeadStarBankUI : MonoBehaviour
{
    public static DeadStarBankUI Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI whiteDwarfText;
    [SerializeField] private TextMeshProUGUI neutronStarText;
    [SerializeField] private TextMeshProUGUI blackHoleText;

    private IPlayer player;

    private void Awake()
    {
        Instance = this;
        CardGameManager.Instance.OnBeginMatch += CardGameManager_OnBeginMatch;
    }
    private void Start()
    {
        
    }
    
    private void CardGameManager_OnBeginMatch(object sender, System.EventArgs e)
    {
        player = CardGameManager.Instance.GetPlayer();
        player.OnSetMyWhiteDwarf += IPlayer_OnSetMyWhiteDwarf;
        player.OnSetMyNeutronStar += IPlayer_OnSetMyNeutronStar;
        player.OnSetMyBlackHole += IPlayer_OnSetMyBlackHole;
    }

    private void IPlayer_OnSetMyBlackHole(object sender, EventArgs e)
    {
        blackHoleText.text = player.GetBlackHole().ToString();
    }

    private void IPlayer_OnSetMyNeutronStar(object sender, EventArgs e)
    {
        neutronStarText.text = player.GetNeutronStar().ToString();
    }

    private void IPlayer_OnSetMyWhiteDwarf(object sender, EventArgs e)
    {
        whiteDwarfText.text = player.GetWhiteDwarf().ToString();
    }

   
}
