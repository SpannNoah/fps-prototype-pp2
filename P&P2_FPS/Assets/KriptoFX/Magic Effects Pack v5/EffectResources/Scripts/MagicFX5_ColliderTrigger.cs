using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicFX5
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class MagicFX5_ColliderTrigger : MonoBehaviour
    {
        public MagicFX5_EffectSettings EffectSettings;

        private void OnCollisionEnter(Collision collision)
        {
            var          hit     = new MagicFX5_EffectSettings.EffectCollisionHit();
            var contact = collision.contacts[0];

            hit.Target   = contact.otherCollider.transform;
            hit.Position = contact.point;
            hit.Normal   = contact.normal;
            EffectSettings.OnEffectCollisionEnter?.Invoke(hit);
        }
    }
}
