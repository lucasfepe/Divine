using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OpponentInfoUI : MonoBehaviour
{
    public static OpponentInfoUI Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI username;
    [SerializeField] private TextMeshProUGUI city;
    [SerializeField] private TextMeshProUGUI country;
    [SerializeField] private TextMeshProUGUI skill;


    private void Awake()
    {
        Instance = this;
    }
    public void Refresh()
    {
        Debug.Log("REFREWSH");
        string opponentusername = "";
        int opponentSkill = 0;
        switch (Player.Instance.IAm())
        {
            case PlayerEnum.PlayerOne:
                foreach (PlayerData data in MatchmakerNetwork.Instance.playerDataNetworkList)
                {
                    if (data.clientId == 2)
                    {
                        opponentusername = data.playerName.ToString();
                        opponentSkill = data.skill;
                    }
                }
                break;
            case PlayerEnum.PlayerTwo:
                foreach (PlayerData data in MatchmakerNetwork.Instance.playerDataNetworkList)
                {
                    if (data.clientId == 1)
                    {
                        opponentusername = data.playerName.ToString();
                        opponentSkill = data.skill;
                    }
                }
                break;
        }
        username.text = opponentusername;
        skill.text = "skill: " + opponentSkill;
    }
}
