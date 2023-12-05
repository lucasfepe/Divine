using System;
using System.Diagnostics;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class MatchmakerNetwork : NetworkBehaviour
{
    public static MatchmakerNetwork Instance { get; private set; }
    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnClientConnected;
    public NetworkList<PlayerData> playerDataNetworkList;



    private AuthenticationManager authenticationManager;
    private void OnDestroy()
    {
        playerDataNetworkList.Dispose();

    }
    public override void OnNetworkSpawn()
    {
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        UnityEngine.Debug.Log("PlayerDataNetworkList_OnListChanged");
        OpponentInfoUI.Instance.Refresh();
    }

    private void Awake()
    {
        playerDataNetworkList = new NetworkList<PlayerData>();
        authenticationManager = FindObjectOfType<AuthenticationManager>();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
    }   
   
            
    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }
    [ServerRpc(RequireOwnership = false)]
    private void AddToPlayerListServerRpc(PlayerData playerData)
    {
        playerDataNetworkList.Add(playerData);
    }
    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    {
        UnityEngine.Debug.Log("NetworkManager_Client_OnClientConnectedCallback");
        AddToPlayerListServerRpc(new PlayerData
        {
            clientId = clientId,
            playerName = authenticationManager.GetUsername(),
            skill = Matchmaker.Instance.GetSkill()

        });

        if (clientId == 1)
            MainMenuToGameStatic.playerNo = 1;
        if (clientId == 2)
            MainMenuToGameStatic.playerNo = 2;
        OnClientConnected?.Invoke(this, EventArgs.Empty);
        //SetPlayerNameServerRpc(GetPlayerName());
        //SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }
    public void StartServer()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartServer();
    }

    private void Singleton_OnClientDisconnectCallback(ulong clientId)
    {

        //for (int i = 0; i < playerDataNetworkList.Count; i++)
        //{
        //    PlayerData playerData = playerDataNetworkList[i];
        //    if (playerData.clientId == clientId)
        //    {
        //        // Disconnected!
        //        playerDataNetworkList.RemoveAt(i);
        //    }
        //}

#if DEDICATED_SERVER
        //Debug.Log("playerDataNetworkList.Count " + playerDataNetworkList.Count);
        if (SceneManager.GetActiveScene().name == SceneLoader.Scene.GameScene.ToString())
        {
            // Player leaving during GameScene
            //if (playerDataNetworkList.Count <= 0)
            //{
            //    // All players left the game
            //    Debug.Log("All players left the game");
            //    Debug.Log("Shutting Down Network Manager");
            //    NetworkManager.Singleton.Shutdown();
            //    Application.Quit();
            //    //Debug.Log("Going Back to Main Menu");
            //    //Loader.Load(Loader.Scene.MainMenuScene);
            //}
        }
#endif
    }

    private void Singleton_OnClientConnectedCallback(ulong clientId)
    {
        //playerDataNetworkList.Add(new PlayerData
        //{
        //    clientId = clientId,

        //});
        
        //SetPlayerNameServerRpc(GetPlayerName());
        //SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != SceneLoader.Scene.GameScene.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= 2)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            return;
        }

        connectionApprovalResponse.Approved = true;
    }

}
