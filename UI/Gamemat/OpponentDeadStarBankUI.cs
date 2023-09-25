using Amazon.DynamoDBv2.DataModel;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class OpponentDeadStarBankUI : MonoBehaviour
{
    public static OpponentDeadStarBankUI Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI blackDwarfText;
    [SerializeField] private TextMeshProUGUI whiteDwarfText;
    [SerializeField] private TextMeshProUGUI neutronStarText;
    [SerializeField] private TextMeshProUGUI blackHoleText;

    private IPlayer player;
    private IPlayer opponent;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        CardGameManager.Instance.OnBeginMatch += CardGameManager_OnBeginMatch;
    }
    //ugly this is so similar of deadstarbank can't you somehow not have two classes that are the same
    //not really because they are listening to different events...
    private void CardGameManager_OnBeginMatch(object sender, System.EventArgs e)
    {
        //I am indicating the opponent information though both the fact that I am referencing the opponent player and the fact of listening to the Opponent event
        //Can't I get rid of one of them?
        //like opponnet.OnSetBlackDwarf....
        //silly boy, the opponent is never going to call it... because this gets called on the player
        //but what if there is a way to skip the event and just call the 'setOpponentBlackDwarf' like directly in the onValueChange
        //i don't think so because on value change happens for both players
        player = CardGameManager.Instance.GetPlayer();
        opponent = CardGameManager.Instance.GetOpponent();
        player.OnSetOpponentBlackDwarf += IPlayer_OnSetOpponentBlackDwarf;
        player.OnSetOpponentWhiteDwarf += IPlayer_OnSetOpponentWhiteDwarf;
        player.OnSetOpponentNeutronStar += IPlayer_OnSetOpponentNeutronStar;
        player.OnSetOpponentBlackHole += IPlayer_OnSetOpponentBlackHole;
    }

    private void IPlayer_OnSetOpponentBlackHole(object sender, System.EventArgs e)
    {
        blackHoleText.text = opponent.GetBlackHole().ToString();
    }

    private void IPlayer_OnSetOpponentNeutronStar(object sender, System.EventArgs e)
    {
        neutronStarText.text = opponent.GetNeutronStar().ToString();
    }

    private void IPlayer_OnSetOpponentWhiteDwarf(object sender, System.EventArgs e)
    {
        whiteDwarfText.text = opponent.GetWhiteDwarf().ToString();
    }

    private void IPlayer_OnSetOpponentBlackDwarf(object sender, System.EventArgs e)
    {
        blackDwarfText.text = opponent.GetBlackDwarf().ToString();
    }


}
