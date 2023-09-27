using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DeadStarBank : MonoBehaviour
{
    public static DeadStarBank Instance { get; private set; }
    
    private List<BaseCard> baseCards;
    private IPlayer player;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        CardGameManager.Instance.OnBeginTurnStep2 += CardGameManger_OnBeginTurn;
        CardGameManager.Instance.OnBeginMatch += CardGameMaanger_OnBeginMatch;
    }

    private void CardGameMaanger_OnBeginMatch(object sender, System.EventArgs e)
    {
        player = CardGameManager.Instance.GetPlayer();
        
    }

    

    private void CardGameManger_OnBeginTurn(object sender, System.EventArgs e)
    {
        baseCards = PlayerPlayingField.Instance.GetAllPlayerExpertCards();
        player.SetLight(player.GetLight() + player.GetNeutronStar() * 10 + player.GetWhiteDwarf() * 5);
        
        for(int i = 0; i < player.GetBlackHole(); i++)
        {
            BlackHoleSuck();
        }
    }

    public void BirthBlackDwarf()
    {
        player.SetBlackDwarf(player.GetBlackDwarf() + 1);
    }

    public void BirthWhiteDwarf() {
        player.SetWhiteDwarf(player.GetWhiteDwarf() + 1);
    }
    public void BirthNeutronStar()
    {
        if (player.GetNeutronStar() == 1)
        {
            player.SetNeutronStar(0);
            
            SpreadStrangeMatter();
            BirthBlackHole();
        }
        else
        {
            player.SetNeutronStar(player.GetNeutronStar() + 1);
        }
    }
    public void BirthBlackHole() {
        player.SetBlackHole(player.GetBlackHole() + 1);
    }

    

    private void SpreadStrangeMatter()
    {
        Status status = new Status();
        status.statusPower = 1;
        status.statusType = EffectTypeEnum.Strange;
        List<BaseCard> myStars = PlayerPlayingField.Instance.GetAllPlayerExpertCards().Where(x => x.IsCardDestroyed() == false).ToList();
        if(myStars.Count > 0) {
            BaseCard strangeTarget = ListUtility.GetRandomItemFromList(myStars);
        strangeTarget.AddStatus(status);
        }
    }
    
    private void BlackHoleSuck()
    {
        int blackHoleSuckLightValue = 1;
        player.SetLight(player.GetLight() - blackHoleSuckLightValue);
        List<BaseCard> aliveCards = baseCards.Where(x => !x.IsCardDestroyed()).ToList();
        if (aliveCards.Count > 0) {
            BaseCard blackHoleTarget = ListUtility.GetRandomItemFromList(aliveCards);
            blackHoleTarget.BlackHoleApproach();
       
        }
    }
}
