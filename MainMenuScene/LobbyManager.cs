using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using Unity.Services.Relay.Models;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using System.Threading.Tasks;
using Unity.Services.Multiplay;

public class LobbyManager : MonoBehaviour
{

    public static LobbyManager Instance { get; private set; }
    private const int MAX_PLAYER_AMOUNT = 2;
    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    private Lobby joinedLobby;

#if DEDICATED_SERVER
    private float autoAllocateTimer = 9999999f;
    private bool alreadyAutoAllocated;
    private static IServerQueryHandler serverQueryHandler; // static so it doesn't get destroyed when this object is destroyed
#endif

    private void Awake()
    {
#if DEDICATED_SERVER
        Debug.Log("DEDICATED_SERVER 6.8");
        
#endif
        if (Instance == null) { 
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(gameObject);
        InitializeUnityAuthentication();
    }
    private void Update()
    {
#if DEDICATED_SERVER
        autoAllocateTimer -= Time.deltaTime;
        if(autoAllocateTimer <= 0f)
        {
            Debug.Log("what111111111");
            autoAllocateTimer = 999f;
            MultiplayEventCallbacks_Allocate(null);

        }
        if(serverQueryHandler != null)
        {

            if (NetworkManager.Singleton.IsServer)
            {
                serverQueryHandler.CurrentPlayers = (ushort)NetworkManager.Singleton.ConnectedClientsIds.Count;
            }
            serverQueryHandler.UpdateServerCheck();
        }
#endif
    }
    private async void InitializeUnityAuthentication()
    {
        if(UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
#if !DEDICATED_SERVER
            initializationOptions.SetProfile(UnityEngine.Random.Range(0,10000).ToString());
#endif
        
        await UnityServices.InitializeAsync(initializationOptions);


#if !DEDICATED_SERVER
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
#endif

#if DEDICATED_SERVER
            Debug.Log("DEDICATED_SERVER LOBBY");
            MultiplayEventCallbacks multiplayEventCallbacks = new MultiplayEventCallbacks();
            multiplayEventCallbacks.Allocate += MultiplayEventCallbacks_Allocate;
            multiplayEventCallbacks.Deallocate += MultiplayEventCallbacks_Deallocate;
            multiplayEventCallbacks.Error += MultiplayEventCallbacks_Error;
            multiplayEventCallbacks.SubscriptionStateChanged += MultiplayEventCallbacks_SubscriptionStateChanged;
            IServerEvents serverEvents = await MultiplayService.Instance.SubscribeToServerEventsAsync(multiplayEventCallbacks);

            serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(4, "MyServerName", "Divine", "1.0", "Default");
            Debug.Log("serverQueryHandler: " + serverQueryHandler);
            var serverConfig = MultiplayService.Instance.ServerConfig;
            if (serverConfig.AllocationId != "")
            {
                // Already Allocated
                MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId, serverConfig.AllocationId));
            }
#endif
        }
        else
        {
            // Already Initialized
#if DEDICATED_SERVER
            Debug.Log("DEDICATED_SERVER LOBBY - ALREADY INIT");

            var serverConfig = MultiplayService.Instance.ServerConfig;
            if (serverConfig.AllocationId != "")
            {
                // Already Allocated
                MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId, serverConfig.AllocationId));
            }
#endif
        }
    }


#if DEDICATED_SERVER
    private void MultiplayEventCallbacks_SubscriptionStateChanged(MultiplayServerSubscriptionState obj)
    {
        Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_SubscriptionStateChanged");
        Debug.Log(obj);
    }

    private void MultiplayEventCallbacks_Error(MultiplayError obj)
    {
        Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_Error");
        Debug.Log(obj.Reason);
    }

    private void MultiplayEventCallbacks_Deallocate(MultiplayDeallocation obj)
    {
        Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_Deallocate");
    }

    private void MultiplayEventCallbacks_Allocate(MultiplayAllocation obj)
    {
        Debug.LogWarning("DEDICATED_SERVER MultiplayEventCallbacks_Allocate");
       
        if (alreadyAutoAllocated)
        {
            Debug.Log("Already auto allocated!");
            return;
        }
        Debug.Log("Already auto allocated2!");
        alreadyAutoAllocated = true;

        var serverConfig = MultiplayService.Instance.ServerConfig;
        //Debug.Log($"Server ID[{serverConfig.ServerId}]");
        //Debug.Log($"AllocationID[{serverConfig.AllocationId}]");
        //Debug.Log($"Port[{serverConfig.Port}]");
        //Debug.Log($"QueryPort[{serverConfig.QueryPort}]");
        //Debug.Log($"LogDirectory[{serverConfig.ServerLogDirectory}]");

        string ipv4Address = "0.0.0.0";
        ushort port = serverConfig.Port;
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port, "0.0.0.0");


        Debug.LogWarning("@@@@@@@@@@@@@ALREADYNOTAUTOALOCATED");
        MatchmakerNetwork.Instance.StartServer();
        Debug.LogWarning("@@@@@@@@@@@@@ALREADYNOTAUTOALOCATED2");
        SceneLoader.LoadNetwork(SceneLoader.Scene.GameScene);
        //SceneLoader.Load(SceneLoader.Scene.GameScene);
    }
#endif

    private async Task<Allocation> AllocateRelay()
    {
        try { 
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MAX_PLAYER_AMOUNT - 1);
            return allocation;
        }catch(RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try { 
        string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        return relayJoinCode;
        }catch(RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    //public async void JoinRanked()
    //{
    //    try
    //    {
    //        joinedLobby = await LobbyService.Instance.CreateLobbyAsync("lobbyName" + UnityEngine.Random.Range(0f, 10000f), MAX_PLAYER_AMOUNT,
                
    //        new CreateLobbyOptions { IsPrivate = false });
    //        Debug.Log("JoinRanked");
    //        Allocation allocation = await AllocateRelay();

    //        string relayJoinCode = await GetRelayJoinCode(allocation);
    //        await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id,
    //            new UpdateLobbyOptions
    //            {
    //                Data = new Dictionary<string, DataObject>
    //                {
    //                    {
    //                        KEY_RELAY_JOIN_CODE, new DataObject(
    //                            DataObject.VisibilityOptions.Member,
    //                            relayJoinCode
    //                        )
    //                    }
    //                }
    //            });

    //        NetworkManager.Singleton.GetComponent<UnityTransport>()
    //            .SetRelayServerData(new RelayServerData(allocation, "dtls"));
    //        IsHostOrClient.SetHostClient(IsHostOrClient.HostClient.Host);
    //        SceneLoader.Load(SceneLoader.Scene.GameScene);
            
            
    //    }
    //    catch (LobbyServiceException ex)
    //    {
    //        Debug.LogException(ex);
    //    }
    //}

    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try { 
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        return joinAllocation;
        }catch(RelayServiceException e)
        {
            Debug.Log(e); return default;
        }
    }

    //public async void QuickJoin()
    //{
    //    try { 
    //    joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
    //        string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
    //        JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
    //        NetworkManager.Singleton.GetComponent<UnityTransport>()
    //            .SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
    //        IsHostOrClient.SetHostClient(IsHostOrClient.HostClient.Client);
    //        SceneLoader.Load(SceneLoader.Scene.GameScene);
    //        Debug.Log("QuickJoin1");

    //    }
    //    catch(LobbyServiceException e)
    //    {
    //        Debug.Log("QuickJoin2");
    //        JoinRanked();
    //    }
    //}
}
