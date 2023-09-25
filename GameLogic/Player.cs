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
    public override void OnNetworkSpawn()
    {
        
        if (IsHost)
        {
            playerEnum = PlayerEnum.PlayerOne;

        }
        else
        {
            playerEnum = PlayerEnum.PlayerTwo;
            //When the second player joins, begin the match
            //CardGameManager.Instance.BeginMatchServerRpc();
            //catch up player one cards when player two joins
            //this doens't feel like the best place to put this
            //OpponentDeckUI.Instance
            //    .DivineMultiplayer_OnDeckCardsNumberChanged(this,
            //    new OnDeckCardsNumberChangedEventArgs
            //    {
            //        playerEnum = PlayerEnum.PlayerOne
            //    });

            //OpponentHandUI.Instance.DivineMultiplayer_OnHandCardsNumberChanged(this,
            //    new OnHandCardsNumberChangedEventArgs
            //    {
            //        playerEnum = PlayerEnum.PlayerOne
            //    });

        }
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
