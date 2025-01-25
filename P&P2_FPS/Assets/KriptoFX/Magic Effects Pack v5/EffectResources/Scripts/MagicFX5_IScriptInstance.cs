using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicFX5
{
    public abstract class MagicFX5_IScriptInstance : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            MagicFX5_GlobalUpdate.CreateInstanceIfRequired();
            MagicFX5_GlobalUpdate.ScriptInstances.Add(this);
            OnEnableExtended();
        }

        protected virtual void OnDisable()
        {
            MagicFX5_GlobalUpdate.ScriptInstances.Remove(this);
            OnDisableExtended();
        }

        internal abstract void OnEnableExtended();
        internal abstract void OnDisableExtended();

        internal abstract void ManualUpdate();
    }
}