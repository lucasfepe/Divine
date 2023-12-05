using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CardSO : ScriptableObject
{
    //This should only be unchanging properties

    //public TalentEnum cardTalentClass;
    public StarClassEnum Rank;
    public string ImageURL;
    public string RedGiantImageURL;
    public string Title;
    public GiantEffect GiantEffect;
    public SupernovaEffect SuperNovaEffect;
    //public CardTypeEnum cardType;
    //public int? progress;
    //public int? opponentProgress;
    public int Lifetime;
    public int Stardust;
    public int Light;
    public int Shed;
    public RarityEnum Rarity;
    public CollectionsEnum Collection;
   

    
}
