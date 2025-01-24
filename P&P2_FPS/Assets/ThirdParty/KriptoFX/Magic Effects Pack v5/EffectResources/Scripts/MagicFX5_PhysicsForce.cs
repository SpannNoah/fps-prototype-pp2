using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MagicFX5
{
    public class MagicFX5_PhysicsForce : MonoBehaviour
    {
        public MagicFX5_EffectSettings EffectSettings;
        public Transform          PhysicsForceEpicenter;

        public float              Delay            = 0;

        void OnEnable()
        {
            if (EffectSettings.UseForce) Invoke("SetForce", Delay);
        }
        void OnDisable()
        {
            if (EffectSettings.UseForce) CancelInvoke("SetForce");
        }

        public void SetForce()
        {
            var allColliders = Physics.OverlapSphere(transform.position, EffectSettings.ForceRadius, EffectSettings.ForceLayerMask);
            if (allColliders.Length == 0) return;

            var epicenter = PhysicsForceEpicenter ? PhysicsForceEpicenter.position : transform.position;

            foreach (var coll in allColliders)
            {
                if (coll.gameObject.isStatic) continue;

                if (coll.attachedRigidbody != null)
                {
                    var direction = coll.transform.position - epicenter;
                    var distance  = direction.magnitude;
                    coll.attachedRigidbody.AddForce(direction.normalized * EffectSettings.Force * (1 - distance / EffectSettings.ForceRadius), ForceMode.Impulse);
                }
            }
        }
    }
}
