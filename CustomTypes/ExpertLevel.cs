using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ExpertLevel 
{
    public int level;
    public TalentValues energyGeneration;
    public int maxHealth;
    public int maxExperience;
    public int tolerance;
    public TalentValues levelUpCost;
    public List<ExpertAction> expertActions;
}
