using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DeadStarBank : MonoBehaviour
{
    public static DeadStarBank Instance { get; private set; }
    
    private List<BaseCard> baseCards;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        CardGameManager.Instance.OnBeginTurnStep2 += CardGameManger_OnBeginTurn;
    }

    private void CardGameManger_OnBeginTurn(object sender, System.EventArgs e)
    {
        baseCards = PlayerPlayingField.Instance.GetAllPlayerExpertCards();
        DivineMultiplayer.Instance.IncreaseLight(DivineMultiplayer.Instance.GetNeutronStarCount() * 10 + DivineMultiplayer.Instance.GetWhiteDwarfCount() * 5);
        
        for(int i = 0; i < DivineMultiplayer.Instance.GetBlackHoleCount(); i++)
        {
            BlackHoleSuck();
        }
    }

    public void BirthBlackDwarf()
    {
        DivineMultiplayer.Instance.IncreaseBlackDwarfCount();
    }

    public void BirthWhiteDwarf() {
        DivineMultiplayer.Instance.IncreaseWhiteDwarfCount();
    }
    public void BirthNeutronStar()
    {
        if (DivineMultiplayer.Instance.GetNeutronStarCount() == 1)
        {
            DivineMultiplayer.Instance.DecreaseNeutronStarCount();
            SpreadStrangeMatter();
            BirthBlackHole();
        }
        else
        {
            DivineMultiplayer.Instance.IncreaseNeutronStarCount();
        }
    }
    public void BirthBlackHole() { DivineMultiplayer.Instance.IncreaseBlackHoleCount(); }

    

    private void SpreadStrangeMatter()
    {
        Status status = new Status();
        status.statusPower = 1;
        status.statusType = EffectTypeEnum.Strange;
        List<BaseCard> myStars = PlayerPlayingField.Instance.GetAllPlayerExpertCards();
        if(myStars.Count > 0) { 
        BaseCard strangeTarget = ListUtility.GetRandomItemFromList(myStars);
        strangeTarget.AddStatus(status);
        }
    }
    //ugly repeated logic with expertcard ln 173
    private void BlackHoleSuck()
    {
        
        int blackHoleSuckLightValue = 1;
        DivineMultiplayer.Instance.DecreaseLight(blackHoleSuckLightValue);
        List<BaseCard> aliveCards = baseCards.Where(x => !x.IsCardDestroyed()).ToList();
        if (aliveCards.Count > 0) { 
        BaseCard blackHoleTarget = ListUtility.GetRandomItemFromList(aliveCards);
        blackHoleTarget.DecreaseCardLight();
        blackHoleTarget.DecreaseCardStardust();
        //lifetime -= 1;
        
        if (blackHoleTarget.GetCardLight() <= 0 || blackHoleTarget.GetStardust() <= 0)
        {
                BirthBlackDwarf();
                DeadStarBankUI.Instance.UpdateDeadStarBankUI();
                Destroy(blackHoleTarget.gameObject);
        }
        }
    }
}
