using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WhiteDwarfAnimator : MonoBehaviour
{
    private Animator animator;
    private BaseCard card;
    //ugly this is pretty much the same as BlackHoleAnimator/neutronstaranimator can't merger?
    private bool moveToBank;
    private Vector2 whiteDwarfBankPosition;
    private Vector2 whiteDwarfOpponentBankPosition;
    private float moveDelay = 1f;
    private float timer = 0f;
    private bool hasBirthed = false;
    private Vector2 targetMine;
    private Vector2 targetOpponent;
    private float acceptableDistance = .02f;
    private BecomeWhiteDwarfAnimation becomeWhiteDwarfAnimation;
    private void Awake()
    {
       
        animator = GetComponent<Animator>();
        
        whiteDwarfBankPosition = GameObject.Find("WhiteDwarfBankIcon").transform.position;
        whiteDwarfOpponentBankPosition = GameObject.Find("WhiteDwarfOpponentBankIcon").transform.position;
    }
    public void SetDestination()
    {
        targetMine = 1.1f * whiteDwarfBankPosition - .1f * (Vector2)transform.position;
        targetOpponent = 1.1f * whiteDwarfOpponentBankPosition - .1f * (Vector2)transform.position;
    }
    private void FixedUpdate()
    {
        if (moveToBank)
        {
            timer += Time.deltaTime;
            if(timer> moveDelay) { 
            
                if (card.GetCardOwner() == Player.Instance.IAm()) {

                    transform.position = Vector2.Lerp((Vector2)transform.position, targetMine, .03f);

                    if ((whiteDwarfBankPosition - (Vector2)transform.position).magnitude < acceptableDistance && !hasBirthed)
                    {
                        hasBirthed = true;
                        Destroy(gameObject);
                        card.Die();
                    }
                }
                else if (card.GetCardOwner() == Player.Instance.OpponentIs())
                {
                    transform.position = Vector2.Lerp((Vector2)transform.position, targetOpponent, .03f);

                    if ((whiteDwarfOpponentBankPosition - (Vector2)transform.position).magnitude < acceptableDistance && !hasBirthed)
                    {
                        Destroy(gameObject);
                        hasBirthed = true;
                    }
                }
                   
                
            }
        }
    }
    public void MoveWhiteDwarfToBank() {
        moveToBank = true;
        float fadeDuration = .01f;
        becomeWhiteDwarfAnimation.FadeOut2(fadeDuration);
    }

    public void SetCard(BaseCard card)
    {
        this.card = card;
        becomeWhiteDwarfAnimation = card.GetComponentInChildren<BecomeWhiteDwarfAnimation>();
    }

}
