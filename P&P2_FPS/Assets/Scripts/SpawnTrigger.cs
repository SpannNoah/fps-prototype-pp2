using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    public List<Spawner> m_spawners = new List<Spawner>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach(Spawner spawner in m_spawners)
            {
                spawner.SpawnTrigger();
            }

            Destroy(gameObject); // Prevents repeat triggers
        }
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
