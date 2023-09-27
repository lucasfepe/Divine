using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TestNetworkObjectCreator : NetworkBehaviour
{
    [SerializeField] private GameObject testNetworkObjectPrefab;

    public override void OnNetworkSpawn()
    {
        if(IsHost)
        SpawnTestObjectServerRpc();
        
    }

    [ServerRpc(RequireOwnership =false)]
    private void SpawnTestObjectServerRpc()
    {
        GameObject instantiatedTestObjectTransform = Instantiate(testNetworkObjectPrefab);
        instantiatedTestObjectTransform.GetComponent<NetworkObject>().Spawn();
    }

}
