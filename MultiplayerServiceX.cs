using System.Collections;
using System.Collections.Generic;
using Unity.Services.Multiplay;
using UnityEngine;

public class MultiplayerServiceX : MonoBehaviour
{


    private async void Start()
    {
#if DEDICATED_SERVER
        Debug.Log("DEDICATED_SERVER GAME SCENE");

        Debug.Log("ReadyServerForPlayersAsync");

        await MultiplayService.Instance.ReadyServerForPlayersAsync();
#endif
    }
}
