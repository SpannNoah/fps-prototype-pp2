using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicFX5
{
    public class MagicFX5_FreezeRotation : MagicFX5_IScriptInstance
    {
        public bool FreezeX = true;
        public bool FreezeY = true;
        public bool FreezeZ = true;

        internal override void OnEnableExtended()
        {
           
        }

        internal override void OnDisableExtended()
        {
          
        }

        internal override void ManualUpdate()
        {
            var currentEuler = transform.eulerAngles;

            if (FreezeX)
            {
                currentEuler.x = 0;
            }
            if (FreezeY)
            {
                currentEuler.y = 0;
            }
            if (FreezeZ)
            {
                currentEuler.z = 0;
            }

            transform.eulerAngles = currentEuler;
        }
    }
}
