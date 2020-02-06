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
    private GameObject[] _powerUp;
    
    private bool _canSpawn = true;

    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(2.5f);

        while(_canSpawn)
        {
            Vector3 enemyPos = new Vector3(Random.Range(-8.5f, 8.5f), 7.0f, 0);
            GameObject spawnedEnemy = Instantiate(_enemyPrefab, enemyPos, Quaternion.identity);
            spawnedEnemy.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(Random.Range(2.0f, 5.0f));
        }

    }

    IEnumerator SpawnPowerup()
    {
        yield return new WaitForSeconds(2.5f);

        while (_canSpawn)
        {
            Vector3 powerUpPos = new Vector3(Random.Range(-8.5f, 8.5f), 7.0f, 0);

            Instantiate(_powerUp[Random.Range(0, _powerUp.Length)], powerUpPos, Quaternion.identity);

            yield return new WaitForSeconds(Random.Range(5.0f, 15.0f));
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
