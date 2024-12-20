using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu]

public class scriptableDeBuff : ScriptableObject
{
    public float Duration;
    public float speedDeBuff;
    public float damageOverTime;
    public bool applyDamageOverTime;    

}