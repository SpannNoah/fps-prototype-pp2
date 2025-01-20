using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LightningUpgradeStun : MonoBehaviour, IAmmoEffect
{
    public void ApplyAmmoEffect(Vector3 hitPosition, Collider hitTarget, AmmoTypeConfig ammoType)
    {
        if(hitTarget.TryGetComponent(out NavMeshAgent agent))
        {
            agent.isStopped = true;
        }

    }
}
