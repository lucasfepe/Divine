using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Materials : MonoBehaviour
{
    public static Materials Instance { get; private set; }
    public  Material yellowEnergyMaterial;
    public  Material blueEnergyMaterial;
    public  Material greenEnergyMaterial;
    public  Material redEnergyMaterial;

    private void Awake()
    {
        Instance = this;
    }
}
