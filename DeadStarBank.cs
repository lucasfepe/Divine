using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DeadStarBank : MonoBehaviour
{
    private Dictionary<BaseCard,int> blackHoleTargets = new Dictionary<BaseCard,int>();

    public static DeadStarBank Instance { get; private set; }
    
    private List<BaseCard> baseCards;
    private IPlayer player;
    private int whiteDwarvesToBeBirthed;
    private int neutronStarsToBeBirthed;
    private int blackHolesToBeBirthed;

    private void Awake()
    {
        CardGameManager.Instance.OnBeginMatch += CardGameMaanger_OnBeginMatch;
#if DEDICATED_SERVER
        Debug.Log("DEDICATED_SERVER 6.8 dead star");

#endif
        Instance = this;
    }
    private void Start()
    {
        
        
        TurnPhaseStateMachine.Instance.OnTurnPhase_BlackHoleSuck += TurnPhaseStateMachine_OnTurnPhase_BlackHoleSuck;
    }

    private void TurnPhaseStateMachine_OnTurnPhase_BlackHoleSuck(object sender, System.EventArgs e)
    {
        blackHoleTargets = new Dictionary<BaseCard, int>();

        baseCards = PlayerPlayingField.Instance.GetAllPlayerExpertCards();

        for (int i = 0; i < player.GetBlackHole(); i++)
        {
            BlackHoleSuck();
        }
        foreach (KeyValuePair<BaseCard, int> blackHoleTarget in blackHoleTargets)
        {
            ((ExpertCard)blackHoleTarget.Key).IncreaseStatDecrement(blackHoleTarget.Value);
            blackHoleTarget.Key.BlackHoleApproach();
        }
        ActionBirthWhiteDwarf();
        ActionBirthNeutronStar();
        ActionBirthBlackHole();
    }

    private void CardGameMaanger_OnBeginMatch(object sender, System.EventArgs e)
    {
        player = CardGameManager.Instance.GetPlayer();

    }

    

    

   

    public void BirthWhiteDwarf() {
        whiteDwarvesToBeBirthed++;
        
    }
    private void ActionBirthWhiteDwarf()
    {
        player.SetWhiteDwarf(player.GetWhiteDwarf() + whiteDwarvesToBeBirthed);
        whiteDwarvesToBeBirthed = 0;
    }
    public void BirthNeutronStar()
    {
        neutronStarsToBeBirthed++;
    }
    private void ActionBirthNeutronStar()
    {
        player.SetNeutronStar(player.GetNeutronStar() + neutronStarsToBeBirthed);
        neutronStarsToBeBirthed = 0;
    }
    public void BirthBlackHole() {
        blackHolesToBeBirthed++;
    }
    private void ActionBirthBlackHole()
    {
        player.SetBlackHole(player.GetBlackHole() + blackHolesToBeBirthed);
        blackHolesToBeBirthed = 0;
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
        
        
        List<BaseCard> aliveCards = baseCards.Where(x => !x.IsCardDestroyed()).ToList();
        if (aliveCards.Count > 0) {
            BaseCard blackHoleTarget = ListUtility.GetRandomItemFromList(aliveCards);
            if(blackHoleTargets.TryGetValue(blackHoleTarget, out _))
            {
                blackHoleTargets[blackHoleTarget]++;
            }
            else
            {
                blackHoleTargets[blackHoleTarget] = 1;
            };
            
        }
    }
}
