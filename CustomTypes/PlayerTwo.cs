using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerTwo : NetworkBehaviour, IPlayer
{
    public NetworkVariable<int> playerTwoStardust = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerTwoLight = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerTwoBlackDwarf = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerTwoWhiteDwarf = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerTwoNeutronStar = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerTwoBlackHole = new NetworkVariable<int>(0);

    public event EventHandler OnSetMyStardust;
    public event EventHandler OnSetOpponentStardust;
    public event EventHandler OnSetMyLight;
    public event EventHandler OnSetOpponentLight;
    public event EventHandler OnSetMyWhiteDwarf;
    public event EventHandler OnSetOpponentWhiteDwarf;
    public event EventHandler OnSetMyBlackDwarf;
    public event EventHandler OnSetOpponentBlackDwarf;
    public event EventHandler OnSetMyNeutronStar;
    public event EventHandler OnSetOpponentNeutronStar;
    public event EventHandler OnSetMyBlackHole;
    public event EventHandler OnSetOpponentBlackHole;

    override public void OnNetworkSpawn()
    {
        playerTwoStardust.OnValueChanged += PlayerTwoStardust_OnValueChanged;
        playerTwoLight.OnValueChanged += PlayerTwoLight_OnValueChanged;
        playerTwoBlackDwarf.OnValueChanged += PlayerTwoBlackDwarf_OnValueChanged;
        playerTwoWhiteDwarf.OnValueChanged += PlayerTwoWhiteDwarf_OnValueChanged;
        playerTwoNeutronStar.OnValueChanged += PlayerTwoNeutronStar_OnValueChanged;
        playerTwoBlackHole.OnValueChanged += PlayerTwoBlackHole_OnValueChanged;
    }

    private void PlayerTwoStardust_OnValueChanged(int previousValue, int newValue)
    {
        CardGameManager.Instance.GetPlayer().UpdateStardustUIPlayerTwo();
    }
    private void PlayerTwoLight_OnValueChanged(int previousValue, int newValue)
    {
        CardGameManager.Instance.GetPlayer().UpdateLightUIPlayerTwo();
    }
    private void PlayerTwoBlackDwarf_OnValueChanged(int previousValue, int newValue)
    {
        CardGameManager.Instance.GetPlayer().UpdateBlackDwarfUIPlayerTwo();
    }
    private void PlayerTwoWhiteDwarf_OnValueChanged(int previousValue, int newValue)
    {
        CardGameManager.Instance.GetPlayer().UpdateWhiteDwarfUIPlayerTwo();
    }
    private void PlayerTwoNeutronStar_OnValueChanged(int previousValue, int newValue)
    {
        CardGameManager.Instance.GetPlayer().UpdateNeutronStarUIPlayerTwo();
    }
    private void PlayerTwoBlackHole_OnValueChanged(int previousValue, int newValue)
    {
        CardGameManager.Instance.GetPlayer().UpdateBlackHoleUIPlayerTwo();
    }
    public void UpdateStardustUIPlayerOne()
    {
        OnSetOpponentStardust?.Invoke(this, EventArgs.Empty);
    }
    public void UpdateStardustUIPlayerTwo()
    {
        OnSetMyStardust?.Invoke(this, EventArgs.Empty);
    }
    public void SetStardust(int newValue)
    {
        SetStardustServerRpc(newValue);
    }

    public int GetStardust()
    {
        return playerTwoStardust.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStardustServerRpc(int newValue)
    {
        playerTwoStardust.Value = newValue;
    }

    public void SetLight(int newValue)
    {
        SetLightServerRpc(newValue);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetLightServerRpc(int newValue)
    {
        playerTwoLight.Value = newValue;
        if (newValue >= UniversalConstants.LIGHT_WIN_THRESHOLD)
        {
            CardGameManager.Instance.MatchEndServerRpc(PlayerEnum.PlayerTwo);
        }
    }

    public int GetLight()
    {
        return playerTwoLight.Value;
    }
    public void UpdateLightUIPlayerOne()
    {
        OnSetOpponentLight?.Invoke(this, EventArgs.Empty);

    }

    public void UpdateLightUIPlayerTwo()
    {
        OnSetMyLight?.Invoke(this, EventArgs.Empty);
    }
    public int GetBlackDwarf()
    {
        return playerTwoBlackDwarf.Value;
    }

    public int GetWhiteDwarf()
    {
        return playerTwoWhiteDwarf.Value;
    }

    public int GetNeutronStar()
    {
        return playerTwoNeutronStar.Value;
    }

    public int GetBlackHole()
    {
        return playerTwoBlackHole.Value;
    }

    public void SetBlackDwarf(int newValue)
    {
        SetBlackDwarfServerRpc(newValue);
    }

    [ServerRpc(RequireOwnership =false)]
    private void SetBlackDwarfServerRpc(int newValue)
    {
        playerTwoBlackDwarf.Value = newValue;
    }

    public void SetWhiteDwarf(int newValue)
    {
        SetWhiteDwarfServerRpc(newValue);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetWhiteDwarfServerRpc(int newValue)
    {
        playerTwoWhiteDwarf.Value = newValue;
    }

    public void SetNeutronStar(int newValue)
    {
        SetNeutronStarServerRpc(newValue);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetNeutronStarServerRpc(int newValue)
    {
        playerTwoNeutronStar.Value = newValue;
    }

    public void SetBlackHole(int newValue)
    {
        SetBlackHoleServerRpc(newValue);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetBlackHoleServerRpc(int newValue)
    {
        playerTwoBlackHole.Value = newValue;
    }

    public void UpdateBlackDwarfUIPlayerOne()
    {
        OnSetOpponentBlackDwarf?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateWhiteDwarfUIPlayerOne()
    {
        OnSetOpponentWhiteDwarf?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateNeutronStarUIPlayerOne()
    {
        OnSetOpponentNeutronStar?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateBlackHoleUIPlayerOne()
    {
        OnSetOpponentBlackHole?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateBlackDwarfUIPlayerTwo()
    {
        OnSetMyBlackDwarf?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateWhiteDwarfUIPlayerTwo()
    {
        OnSetMyWhiteDwarf?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateNeutronStarUIPlayerTwo()
    {
        OnSetMyNeutronStar?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateBlackHoleUIPlayerTwo()
    {
        OnSetMyBlackHole?.Invoke(this, EventArgs.Empty);
    }
}
