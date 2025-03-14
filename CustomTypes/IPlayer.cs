using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IPlayer 
{
    void SetStardust(int newValue);
    int GetStardust();
    void SetLight(int newValue);
    int GetLight();
    void SetWhiteDwarf(int newValue);
    void SetNeutronStar(int newValue);
    void SetBlackHole(int newValue);
    int GetWhiteDwarf();
    int GetNeutronStar();
    int GetBlackHole();

    void UpdateStardustUIPlayerOne();
    void UpdateStardustUIPlayerTwo();
    void UpdateLightUIPlayerOne();
    void UpdateLightUIPlayerTwo();
    void UpdateWhiteDwarfUIPlayerOne();
    void UpdateNeutronStarUIPlayerOne();
    void UpdateBlackHoleUIPlayerOne();
    void UpdateWhiteDwarfUIPlayerTwo();
    void UpdateNeutronStarUIPlayerTwo();
    void UpdateBlackHoleUIPlayerTwo();

    public event EventHandler OnSetMyStardust;
    public event EventHandler OnSetOpponentStardust;
    public event EventHandler OnSetMyLight;
    public event EventHandler OnSetOpponentLight;
    public event EventHandler OnSetMyWhiteDwarf;
    public event EventHandler OnSetOpponentWhiteDwarf;
    public event EventHandler OnSetMyNeutronStar;
    public event EventHandler OnSetOpponentNeutronStar;
    public event EventHandler OnSetMyBlackHole;
    public event EventHandler OnSetOpponentBlackHole;

}
