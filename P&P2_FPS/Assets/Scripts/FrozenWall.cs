using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenWall : MonoBehaviour, IDamage
{
    [SerializeField] private float m_forceRadius = 0.0f;
    [SerializeField] private LayerMask m_ignoreMask;
    [SerializeField] private float m_force = 0.0f;
    [SerializeField] private List<GameObject> m_ParticleSystems = new List<GameObject>();
    public void TakeDamage(int amount)
    {
        foreach(GameObject ps in m_ParticleSystems)
        {
            Instantiate(ps, gameObject.transform.position, Quaternion.identity);
        }
        var allColliders = Physics.OverlapSphere(transform.position, m_forceRadius, ~m_ignoreMask);
        if (allColliders.Length == 0) return;

        var epicenter = transform.position;

        foreach (var coll in allColliders)
        {
            if (coll.gameObject.isStatic) continue;

            if (coll.attachedRigidbody != null)
            {
                var direction = coll.transform.position - epicenter;
                var distance = direction.magnitude;
                coll.attachedRigidbody.AddForce(direction.normalized * m_force * (1 - distance / m_forceRadius), ForceMode.Impulse);
            }
        }

        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
