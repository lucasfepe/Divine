using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ExpertAction 
{
    public string Description;
    public EffectTypeEnum Effect;
    public ViableTarget ViableTarget;
    public int NumberTargets;
    public PreferenceEnum Preference;
}
