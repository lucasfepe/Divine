using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class KillOnSupernovaStop : MonoBehaviour
{
    private BaseCard card;
    private float timer;
    private float timerMax = 1f;
    private bool waitToDie = false;

    private void Update()
    {
        if (waitToDie) { 
        timer += Time.deltaTime;
        if(timer > timerMax)
        {
                card.Die();
            
        }
        }
    }

    public void SetCard(BaseCard card)
    {
        this.card = card;
    }
    public void OnParticleSystemStopped()
    {
        if (card.GetCardOwner() == Player.Instance.IAm())
        {
            //the opponent kills your card not you when absorbing

            waitToDie = true;
        }
    }
}
