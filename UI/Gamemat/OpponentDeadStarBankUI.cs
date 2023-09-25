using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OpponentDeadStarBankUI : MonoBehaviour
{
    public static OpponentDeadStarBankUI Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI blackDwarfText;
    [SerializeField] private TextMeshProUGUI whiteDwarfText;
    [SerializeField] private TextMeshProUGUI neutronStarText;
    [SerializeField] private TextMeshProUGUI blackHoleText;
    private void Awake()
    {
        Instance = this;
    }
    public void UpdateDeadStarBankUI()
    {
        //ugly isn't there a way to do this without the if like based on maybe assign a class to 
        if (Player.Instance.OpponentIs() == PlayerEnum.PlayerOne)
        {
            blackDwarfText.text = DivineMultiplayer.Instance.playerOneBlackDwarf.Value.ToString();
            whiteDwarfText.text = DivineMultiplayer.Instance.playerOneWhiteDwarf.Value.ToString();
            neutronStarText.text = DivineMultiplayer.Instance.playerOneNeutronStar.Value.ToString();
            blackHoleText.text = DivineMultiplayer.Instance.playerOneBlackHole.Value.ToString();
        }
        else if (Player.Instance.OpponentIs() == PlayerEnum.PlayerTwo)
        {
            blackDwarfText.text = DivineMultiplayer.Instance.playerTwoBlackDwarf.Value.ToString();
            whiteDwarfText.text = DivineMultiplayer.Instance.playerTwoWhiteDwarf.Value.ToString();
            neutronStarText.text = DivineMultiplayer.Instance.playerTwoNeutronStar.Value.ToString();
            blackHoleText.text = DivineMultiplayer.Instance.playerTwoBlackHole.Value.ToString();
        }

    }

}
