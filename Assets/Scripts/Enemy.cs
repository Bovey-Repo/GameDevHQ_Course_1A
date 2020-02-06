using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 4.0f;

    [SerializeField]
    private int _enemyValue = 10;

    [SerializeField]
    private GameObject _laserPrefab;

    private Player _player;
    private Animator _animator;
    private AudioSource _audioSource;
    private bool _isAlive = true;

    private void Start()
    {
         _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Enemy: Player is null");
        }

        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Enemy: Animator is null");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_animator == null)
        {
            Debug.LogError("Enemy: AudioSorce is null");
        }

        StartCoroutine(FireLaser());
    }
    void Update()
    {
        transform.Translate(Vector3.down *_enemySpeed * Time.deltaTime);
        if (transform.position.y < -7.0f && _isAlive)
        {
            transform.position = new Vector3(Random.Range(-8.5f, 8.5f), 7.0f, transform.position.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser" && _isAlive)
        {
            _player.IncrementScore(_enemyValue);
            Destroy(other.gameObject);
            DestroyEnemy();
        }

        if (other.tag == "Player")
        {
            _player.Damage();
            if (_isAlive)
            {
                DestroyEnemy();
            }

        }
    }

    private void DestroyEnemy()
    {
        _isAlive = false;
        _enemySpeed = 0;
        _animator.SetTrigger("OnKilled"); // trigger explosion animation
        _audioSource.Play();
        Destroy(this.gameObject, 2.5f);
    }

    IEnumerator FireLaser()
    {
        Instantiate(_laserPrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(Random.Range(1, 5));
    }
}
