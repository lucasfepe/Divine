using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using UnityEngine;

public class ListUtility 
{
    public static T GetRandomItemFromList<T>(List<T> list)
    {
        int randomNumber = new System.Random().Next(list.Count);
        

        return list[randomNumber];
    }
}
