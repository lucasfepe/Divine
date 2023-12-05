using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IsHostOrClient
{
    public enum HostClient
    {
        Null,
        Host,
        Client
    }

    private static HostClient hostClient;
    public static void SetHostClient(HostClient hostClienta)
    {
        hostClient = hostClienta;
    }
    public static HostClient GetHostClient()
    {
        return hostClient;
    }


}
