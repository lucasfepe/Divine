using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Matchmaker;
using UnityEngine;
using UnityEngine.UI;

public class Matchmaker : MonoBehaviour
{
    public const string DEFAULT_QUEUE = "DivineQueue";
    public static Matchmaker Instance { get; private set; } 

    [SerializeField] private Button findMatchButton;
    [SerializeField] private Transform lookingForMatchTransform;
    [SerializeField] private MatchFindStatusUI findMatchStatusUI;
    [SerializeField] private SelectDeckBackButton backDeckButton;

    private CreateTicketResponse createTicketResponse;
    private float pollTicketTimer;
    private float pollTicketTimerMax = 1.5f;
    private int skill;
    private AuthenticationManager authenticationManager;
    private async void Awake()
    {
        Instance = this;
        authenticationManager = FindObjectOfType<AuthenticationManager>();
        lookingForMatchTransform.gameObject.SetActive(false);
        skill = await LambdaManager.Instance.GetPlayerSkillLambda();
        findMatchButton.onClick.AddListener(() => {
            Debug.Log("settingacrtive");
            findMatchStatusUI.gameObject.SetActive(true);
            findMatchStatusUI.SetText("Searching for\nOpponent...");
            FindMatch();
        });
        
        backDeckButton.OnSelectDeckBackButtonPressed += BackDeckButton_OnSelectDeckBackButtonPressed;
    }

    private void BackDeckButton_OnSelectDeckBackButtonPressed(object sender, EventArgs e)
    {
        if(createTicketResponse != null && createTicketResponse.Id != null)
        MatchmakerService.Instance.DeleteTicketAsync(createTicketResponse.Id);
        createTicketResponse = null;
        findMatchStatusUI.gameObject.SetActive(false);
    }

    public int GetSkill()
    {
        return skill;
    }
    private async void FindMatch()
    {
        Debug.Log("FindMatch");

        lookingForMatchTransform.gameObject.SetActive(true);

        createTicketResponse = await MatchmakerService.Instance.CreateTicketAsync(new List<Unity.Services.Matchmaker.Models.Player> {
             new Unity.Services.Matchmaker.Models.Player(AuthenticationService.Instance.PlayerId,
             new MatchmakingPlayerData {
                  Skill = skill,
                  Username = authenticationManager.GetUsername(),
                  
             })
        }, new CreateTicketOptions { QueueName = DEFAULT_QUEUE });

        // Wait a bit, don't poll right away
        pollTicketTimer = pollTicketTimerMax;
    }

    [Serializable]
    public class MatchmakingPlayerData
    {
        public int Skill;
        public string Username;
    }
    private void Start()
    {
        findMatchStatusUI.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (createTicketResponse != null)
        {
            // Has ticket
            pollTicketTimer -= Time.fixedDeltaTime;
            if (pollTicketTimer <= 0f)
            {
                pollTicketTimer = pollTicketTimerMax;

                PollMatchmakerTicket();
            }
        }
    }

    private async void PollMatchmakerTicket()
    {
        Debug.Log("PollMatchmakerTicket");
        TicketStatusResponse ticketStatusResponse = await MatchmakerService.Instance.GetTicketAsync(createTicketResponse.Id);

        if (ticketStatusResponse == null)
        {
            // Null means no updates to this ticket, keep waiting
            Debug.Log("Null means no updates to this ticket, keep waiting");
            return;
        }

        // Not null means there is an update to the ticket
        if (ticketStatusResponse.Type == typeof(MultiplayAssignment))
        {
            // It's a Multiplay assignment
            MultiplayAssignment multiplayAssignment = ticketStatusResponse.Value as MultiplayAssignment;

            



            Debug.Log("multiplayAssignment.Status " + multiplayAssignment.Status);
            switch (multiplayAssignment.Status)
            {
                case MultiplayAssignment.StatusOptions.Found:
                    findMatchStatusUI.SetText("Opponent found!");


                    createTicketResponse = null;

                    Debug.Log(multiplayAssignment.Ip + " " + multiplayAssignment.Port);

                    string ipv4Address = multiplayAssignment.Ip;
                    ushort port = (ushort)multiplayAssignment.Port;
                    Debug.Log("PORT: " + port);
                    NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port);

                    MatchmakerNetwork.Instance.StartClient();
                    //SceneLoader.Load(SceneLoader.Scene.GameScene);
                    break;
                case MultiplayAssignment.StatusOptions.InProgress:
                    findMatchStatusUI.SetText("Searching for\nOpponent...");
                    // Still waiting...
                    break;
                case MultiplayAssignment.StatusOptions.Failed:
                    createTicketResponse = null;
                    Debug.Log("Failed to create Multiplay server!");
                    findMatchStatusUI.SetText("Couldn't find opponent, please try again.");
                    lookingForMatchTransform.gameObject.SetActive(false);
                    break;
                case MultiplayAssignment.StatusOptions.Timeout:
                    createTicketResponse = null;
                    Debug.Log("Multiplay Timeout!");
                    findMatchStatusUI.SetText("Couldn't find opponent, please try again.");
                    lookingForMatchTransform.gameObject.SetActive(false);
                    break;
            }
        }

    }



}
