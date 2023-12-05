using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutronStarAnimator : MonoBehaviour
{

    private bool moveToBank;
    private float moveDelay = 1f;
    private float timer = 0f;
    private bool amOwner = false;
    private Vector2 neutronStarBankPosition;
    private Vector2 neutronStarOpponentBankPosition;
    private bool hasBirthed = false;
    private Vector2 targetMine;
    private Vector2 targetOpponent;
    private float acceptableDistance = .01f;

    private void Awake()
    {
        neutronStarBankPosition = GameObject.Find("NeutronStarBankIcon").transform.position;
        neutronStarOpponentBankPosition = GameObject.Find("NeutronStarOpponentBankIcon").transform.position;
    }
    public void SetDestination()
    {
        targetMine = 1.1f * neutronStarBankPosition - .1f * (Vector2)transform.position;
        targetOpponent = 1.1f * neutronStarOpponentBankPosition - .1f * (Vector2)transform.position;
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

                    transform.position = Vector2.Lerp((Vector2)transform.position, targetMine, .03f);

                    if ((neutronStarBankPosition - (Vector2)transform.position).magnitude < acceptableDistance && !hasBirthed)
                    {
                        hasBirthed = true;
                        
                        Destroy(gameObject);
                    }
                }
                else if (!amOwner)
                {
                    transform.position = Vector2.Lerp((Vector2)transform.position, targetOpponent, .03f);

                    if ((neutronStarOpponentBankPosition - (Vector2)transform.position).magnitude < acceptableDistance && !hasBirthed)
                    {
                        hasBirthed = true;
                        
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
    public void MoveNeutronStarToBank()
    {
        moveToBank = true;

    }

    public void SetOwner(bool amOwner)
    {
        this.amOwner = amOwner;
    }

}
