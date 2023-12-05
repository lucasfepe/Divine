using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusInfo : MonoBehaviour
{
    private EffectTypeEnum effectType;

    public void SetStatusType(EffectTypeEnum effectType)
    {
        this.effectType = effectType;
    }

    public EffectTypeEnum GetStatusType()
    {
        return effectType;
    }

}
