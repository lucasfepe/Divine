using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GiantEffect
{
    public string Description;
    public EffectTypeEnum Effect;
    public PassiveActiveEnum PassiveActive;
    public ViableTarget ViableTarget;
    public int NumberTargets;
    public PreferenceEnum Preference;
    public int Power;
    public int StardustCost;
    public string Twin;
}
