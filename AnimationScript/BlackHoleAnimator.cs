using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleAnimator : MonoBehaviour
{

    private bool moveToBank;
    private float moveDelay = 1f;
    private float timer = 0f;
    private bool amOwner = false;
    private Vector2 blackHoleBankPosition;
    private Vector2 blackHoleOpponentBankPosition;
    private Vector2 targetMine;
    private Vector2 targetOpponent;
    private bool hasBirthed = false;
    private bool isStationary = false;
    private BlackHoleCircleAnimator blackHoleCircleAnimator;
    private BlackHoleAuraAnimator blackHoleAuraAnimator;
    private BaseCard card;
    private bool justKilled = false;
    private float acceptableDistance = .01f;
    private int decrement = 0;


    public event EventHandler OnBlackHoleSuck;

    private void Awake()
    {
        blackHoleBankPosition = GameObject.Find("BlackHoleBankIcon").transform.position;
        blackHoleOpponentBankPosition = GameObject.Find("BlackHoleOpponentBankIcon").transform.position;
        blackHoleCircleAnimator = GetComponentInChildren<BlackHoleCircleAnimator>();
        blackHoleAuraAnimator = GetComponentInChildren<BlackHoleAuraAnimator>();
        

        

    }
    public void SetDestination()
    {
        targetMine = 1.1f * blackHoleBankPosition - .1f * (Vector2)transform.position;
        targetOpponent = 1.1f * blackHoleOpponentBankPosition - .1f * (Vector2)transform.position;
    }
    public void SetCard(BaseCard card)
    {
        this.card = card;
        card.SetBlackHole(this);
    }

    private void FixedUpdate()
    {
        if (moveToBank)
        {
            timer += Time.deltaTime;
            if (timer > moveDelay)
            {

                if (amOwner)
                {
                    if (!hasBirthed) { 
                   transform.position = Vector2.Lerp((Vector2)transform.position, targetMine, .03f);
                    }
                    if ((blackHoleBankPosition - (Vector2)transform.position).magnitude < acceptableDistance && !hasBirthed)
                    {

                        hasBirthed = true;
                        Shrink();
                    }
                }
                else if (!amOwner)
                {
                    if (!hasBirthed)
                    {
                        transform.position = Vector2.Lerp((Vector2)transform.position, targetOpponent, .03f);
                    }
                    if ((blackHoleOpponentBankPosition - (Vector2)transform.position).magnitude < acceptableDistance && !hasBirthed)
                    {
                        hasBirthed = true;
                        Shrink();
                    }
                }
            }
        }
    }

   
    public void MakeStationary()
    {
        isStationary = true;
    }
    public void JustKilled()
    {
        isStationary = true;
        justKilled = true;
    }
    public void MoveBlackHoleToBank()
    {
        if (!isStationary) { 
        moveToBank = true;
        }else if (justKilled)
        {
            StartCoroutine(ShrinkAwayAfterKillCard());
        }
        else
        {
            StartCoroutine(ShrinkAway());
        }
    }

    private IEnumerator ShrinkAway()
    {

        OnBlackHoleSuck?.Invoke(this, EventArgs.Empty);
        

        yield return new WaitForSeconds(1);

        blackHoleCircleAnimator.Disappear();
        blackHoleAuraAnimator.Disappear();
    }

    public IEnumerator ShrinkAwayAfterKillCard()
    {

        yield return new WaitForSeconds(2);

        blackHoleCircleAnimator.Disappear();
        blackHoleAuraAnimator.Disappear();
    }

    private void Shrink()
    {
        blackHoleCircleAnimator.Disappear();
        blackHoleAuraAnimator.Disappear();
    }

    public void SetOwner(bool amOwner)
    {
        this.amOwner = amOwner;
    }

    

}
