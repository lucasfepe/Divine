using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;


public class ExpertCardLocal : BaseCardLocal
{
    

    private int level = 1;
    
    private int lifetime;
    private int experience;
    public event EventHandler OnLevelUp;
    private int health;
    private bool isGiant = false;



    override protected void Awake()
    {
        base.Awake();
        experience = 0;
        
        level = 1;
        
        cardUI = GetComponentInChildren<CardUI>();
        //RefreshBaseEnergyGeneration();
    }

    override  public void SetCardSO(CardSO cardSO)
    {
       base.SetCardSO(cardSO);
        lifetime = cardSO.Lifetime;

        CallOnCardSOAssigned();
    }

    
    private void CheckAlive()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void TakeDamage(int damage)
    {
        this.health -= damage;
        CheckAlive();
    }

    override public ViableTarget GetViableTarget()
    {

        return cardSO.GiantEffect.ViableTarget;
    }

    //public ExpertLevel GetExpertLevel()
    //{
        //return cardSO.expertStatsObjectFromJson.levels[level - 1];
    //}
    override public void DoneInspectingCard()
    {
        base.DoneInspectingCard();
        //levelUpMenuUI.Hide();
        //((ExpertCardUI)cardUI).HideSkills();
    }
    //ugly I am doing this kind of thing in expertcardui too
    override public void InspectCard()
    {
        base.InspectCard();
        //((ExpertCardUI)cardUI).ShowSkills();
    }
    public override bool IsGiant()
    {
        return isGiant;  
    }
    override public int GetLifetime()
    {
        return lifetime;
    }

}


