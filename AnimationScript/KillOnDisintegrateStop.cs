using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOnDisintegrateStop : MonoBehaviour
{
    private BaseCard card;
    public void SetCard(BaseCard card)
    {
        this.card = card;
    }
    public void OnParticleSystemStopped()
    {
        if (card.GetCardOwner() == Player.Instance.OpponentIs())
        {
            //the opponent kills your card not you when absorbing
            card.Die();
        }
    }
}
