using Amazon.DynamoDBv2.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using static PlayerHand;

public class OpponentPlayingField : MonoBehaviour
{
    public static OpponentPlayingField Instance { get; private set; }
    [SerializeField] private List<Transform> expertCardPositions;
    [SerializeField] private List<Transform> civilizationCardPositions;
    [SerializeField] private Transform subterfugeCardPosition;
    [SerializeField] private BaseCard subterfugeCardTemplate;
    

    private float subterfugeCardDeleteDelayTimer = 0f;
    private float subterfugeCardDeleteDelayTimerMax = 2f;
    private bool subterfugeCardPlayed = false;
    private BaseCard subterfugeCard;
    
    private void Awake()
    {
        Instance = this;
    }
   

    public Transform GetFirstOpenExpertCardPosition()
    {
        foreach (Transform t in expertCardPositions)
        {
            if (t.childCount == 0)
            {
                return t;
            }
        }
        return null;
    }

    

    //private void Update()
    //{
    //    if (subterfugeCardPlayed)
    //    {
    //        subterfugeCardDeleteDelayTimer += Time.deltaTime;
    //        if (subterfugeCardDeleteDelayTimer >= subterfugeCardDeleteDelayTimerMax)
    //        {
    //            subterfugeCardDeleteDelayTimer = 0;
    //            subterfugeCardPlayed = false;
    //            Destroy(subterfugeCard);
    //        }
    //    }
    //}
    public List<BaseCard> GetAllOpponentExpertCards()
    {
        List<BaseCard> expertCards = new List<BaseCard>();
        foreach (Transform expertCardPosition in expertCardPositions)
        {
            if (expertCardPosition.childCount > 0)
            {
                //Should only have one!
                expertCards.Add(expertCardPosition.GetChild(0).GetComponent<BaseCard>());
            }
        }
        return expertCards;
    }
}
