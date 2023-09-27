using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerOne : NetworkBehaviour, IPlayer
{
    //ugly do i really nee "One" in all of these?
    //can I have two network variables in two separate classes with the same name?
    
    public NetworkVariable<int> playerOneStardust = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerOneLight = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerOneBlackDwarf = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerOneWhiteDwarf = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerOneNeutronStar = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerOneBlackHole = new NetworkVariable<int>(0);

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
        playerOneStardust.OnValueChanged += PlayerOneStardust_OnValueChanged;
        playerOneLight.OnValueChanged += PlayerOneLight_OnValueChanged;
        playerOneBlackDwarf.OnValueChanged += PlayerOneBlackDwarf_OnValueChanged;
        playerOneWhiteDwarf.OnValueChanged += PlayerOneWhiteDwarf_OnValueChanged;
        playerOneNeutronStar.OnValueChanged += PlayerOneNeutronStar_OnValueChanged;
        playerOneBlackHole.OnValueChanged += PlayerOneBlackHole_OnValueChanged;
    }

    private void PlayerOneStardust_OnValueChanged(int previousValue, int newValue)
    {
        CardGameManager.Instance.GetPlayer().UpdateStardustUIPlayerOne();
    }
    private void PlayerOneLight_OnValueChanged(int previousValue, int newValue)
    {
        CardGameManager.Instance.GetPlayer().UpdateLightUIPlayerOne();
    }
    private void PlayerOneBlackDwarf_OnValueChanged(int previousValue, int newValue)
    {
        CardGameManager.Instance.GetPlayer().UpdateBlackDwarfUIPlayerOne();
    }
    private void PlayerOneWhiteDwarf_OnValueChanged(int previousValue, int newValue)
    {
        CardGameManager.Instance.GetPlayer().UpdateWhiteDwarfUIPlayerOne();
    }
    private void PlayerOneNeutronStar_OnValueChanged(int previousValue, int newValue)
    {
        CardGameManager.Instance.GetPlayer().UpdateNeutronStarUIPlayerOne();
    }
    private void PlayerOneBlackHole_OnValueChanged(int previousValue, int newValue)
    {
        CardGameManager.Instance.GetPlayer().UpdateBlackHoleUIPlayerOne();
    }
    public void UpdateStardustUIPlayerOne()
    {
        //need a way to delay this for beginning of turn purposes
        OnSetMyStardust?.Invoke(this, EventArgs.Empty);
        
    }
    public void UpdateLightUIPlayerOne()
    {
        OnSetMyLight?.Invoke(this, EventArgs.Empty);

    }
    public void UpdateStardustUIPlayerTwo()
    {
        OnSetOpponentStardust?.Invoke(this, EventArgs.Empty);
    }
    public void UpdateLightUIPlayerTwo()
    {
        OnSetOpponentLight?.Invoke(this, EventArgs.Empty);
    }
    public void UpdateBlackDwarfUIPlayerOne()
    {
        OnSetMyBlackDwarf?.Invoke(this, EventArgs.Empty);
    }
    public void UpdateBlackDwarfUIPlayerTwo()
    {
        OnSetOpponentBlackDwarf?.Invoke(this, EventArgs.Empty);
    }

    //ugly this is literally duplicated in playerTwo
    //but like you need it in both places so idk
    public void SetStardust(int newValue)
    {
        SetStardustServerRpc(newValue);
    }
    public int GetStardust()
    {
        return playerOneStardust.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStardustServerRpc(int newValue)
    {
        playerOneStardust.Value = newValue;
    }

    public void SetLight(int newValue)
    {
        SetLightServerRpc(newValue);
    }

    [ServerRpc(RequireOwnership =false)]
    private void SetLightServerRpc(int newValue)
    {
        playerOneLight.Value = newValue;
    }

    public int GetLight()
    {
        return playerOneLight.Value;
    }

    public void SetBlackDwarf(int newValue)
    {
        SetBlackDwarfServerRpc(newValue);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetBlackDwarfServerRpc(int newValue)
    {
        playerOneBlackDwarf.Value = newValue;
    }

    public void SetWhiteDwarf(int newValue)
    {
        SetWhiteDwarfServerRpc(newValue);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetWhiteDwarfServerRpc(int newValue)
    {
        playerOneWhiteDwarf.Value = newValue;
    }

    public void SetNeutronStar(int newValue)
    {
        SetNeutronStarServerRpc(newValue);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetNeutronStarServerRpc(int newValue)
    {
        playerOneNeutronStar.Value = newValue;
    }

    public void SetBlackHole(int newValue)
    {
        SetBlackHoleServerRpc(newValue);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetBlackHoleServerRpc(int newValue)
    {
        playerOneBlackHole.Value = newValue;
    }

    public int GetBlackDwarf()
    {
        return playerOneBlackDwarf.Value;
    }

    public int GetWhiteDwarf()
    {
        return playerOneWhiteDwarf.Value;
    }

    public int GetNeutronStar()
    {
        return playerOneNeutronStar.Value;
    }

    public int GetBlackHole()
    {
        return playerOneBlackHole.Value;
    }

    public void UpdateWhiteDwarfUIPlayerOne()
    {
        OnSetMyWhiteDwarf?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateNeutronStarUIPlayerOne()
    {
        OnSetMyNeutronStar?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateBlackHoleUIPlayerOne()
    {
        OnSetMyBlackHole?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateWhiteDwarfUIPlayerTwo()
    {
        OnSetOpponentWhiteDwarf?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateNeutronStarUI()
    {
        OnSetOpponentNeutronStar?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateBlackHoleUIPlayerTwo()
    {
        OnSetOpponentBlackHole?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateNeutronStarUIPlayerTwo()
    {
        OnSetOpponentNeutronStar?.Invoke(this, EventArgs.Empty);
    }
}
