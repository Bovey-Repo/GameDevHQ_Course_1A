using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private PowerUpObj[] _powerUps;
    
    private List<PowerUpObj> pList = new List<PowerUpObj>();
    private bool _canSpawn = true;
    private int _chance;

    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(2.5f); // delay the spawning for 2.5 seconds after game start

        while(_canSpawn)
        {
            Vector2 enemyPos = new Vector2(Random.Range(-8.5f, 8.5f), 7.0f);
            GameObject spawnedEnemy = Instantiate(_enemyPrefab, enemyPos, Quaternion.identity);
            spawnedEnemy.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(Random.Range(2.0f, 5.0f));
        }

    }

    IEnumerator SpawnPowerup()
    {
        yield return new WaitForSeconds(2.5f); // delay the spawning for 2.5 seconds after game start

        while (_canSpawn)
        {
            ChoosePowerUps();

            GameObject powerUpToSpawn = pList[Random.Range(0, pList.Count)].prefab;
            Vector2 powerUpPos = new Vector2(Random.Range(-8.5f, 8.5f), 7.0f);

            Instantiate(powerUpToSpawn, powerUpPos, Quaternion.identity);
            pList.Clear();

            yield return new WaitForSeconds(Random.Range(5.0f, 15.0f));
        }
    }

    private void ChoosePowerUps()
    {
        int randomChance = Random.Range(1, 101);

        if (randomChance >= 1 && randomChance <= 10)
        {
            _chance = 10;
        }
        else if (randomChance >= 11 && randomChance <= 50)
        {
            _chance = 40;
        }
        else
        {
            _chance = 50;
        }

        foreach (var p in _powerUps)
        {
            if (_chance == p.chance)
            {
                pList.Add(p);
            }
        }
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpawnPowerup());
    }

    public void StopSpawning()
    {
        _canSpawn = false;
    }
}
