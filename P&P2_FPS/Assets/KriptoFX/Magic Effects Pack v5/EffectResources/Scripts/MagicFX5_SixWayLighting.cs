using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicFX5
{
    public class MagicFX5_SixWayLighting : MonoBehaviour
    {
      
        void Awake()
        {
           
        }

        void OnEnable()
        {
            MagicFX5_GlobalUpdate.CreateInstanceIfRequired();
            MagicFX5_GlobalUpdate.SixWayLightingInstances.Add(this);
        }

        void OnDisable()
        {
            MagicFX5_GlobalUpdate.SixWayLightingInstances.Remove(this);
        }

     
    }
}
