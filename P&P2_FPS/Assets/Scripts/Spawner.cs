using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] int timeBetweenSpawns;
    [SerializeField] float spawnRadius;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] bool isTriggeredBySomethingElse = false;

    int spawnCount;

    bool StartSpawning;
    bool isSpawning;
    // Start is called before the first frame update
    void Start()
    {
        if (isTriggeredBySomethingElse) return;

        GameManager.Instance.UpdateGameGoal(numToSpawn);
        StartSpawning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isTriggeredBySomethingElse && !StartSpawning)
        {
            return;
        }

        if (StartSpawning && spawnCount < numToSpawn && !isSpawning)
        {
            StartCoroutine(Spawn());
        }
    }

    IEnumerator Spawn()
    {
        isSpawning = true;
        yield return new WaitForSeconds(timeBetweenSpawns);

        int spawnInt = Random.Range(0, spawnPos.Length);

        Instantiate(objectToSpawn, spawnPos[spawnInt].position, Quaternion.identity);
        spawnCount++;

        isSpawning = false;

    }

    public void SpawnTrigger()
    {
        StartSpawning = true;
    }
}
