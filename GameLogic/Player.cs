using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static DivineMultiplayer;

public class Player : NetworkBehaviour
{
    public static Player Instance { get; private set; }

    private PlayerEnum playerEnum;

    private void Awake()
    {
        Instance = this;
    }
   
   

    public void SetPlayerEnum(PlayerEnum playerEnum)
    {
        this.playerEnum = playerEnum;
    }
    public PlayerEnum IAm() {
        return playerEnum;
    }
    public PlayerEnum OpponentIs()
    {
        if (playerEnum == PlayerEnum.PlayerOne) { return PlayerEnum.PlayerTwo; }
        else if(playerEnum == PlayerEnum.PlayerTwo) { return PlayerEnum.PlayerOne; }
        return PlayerEnum.Null;
    }

}
