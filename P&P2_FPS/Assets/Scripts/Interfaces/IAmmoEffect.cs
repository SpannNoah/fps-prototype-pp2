using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAmmoEffect
{
    void ApplyAmmoEffect(Vector3 hitPosition, Collider hitTarget);
}
