﻿using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;
    private float _speedBoost = 3.5f;
    private float _thrusterBoost = 4.5f;
    [SerializeField]
    private float _thruster = 10.0F;
    [SerializeField]
    private float _thrusterRate = 7.5f;
    private bool _thrusterOn = false;
    private bool _thrusterCharged = true;

    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManger;
    private UIManager _uiManager;
    [SerializeField]
    private int _score;
    private CameraShake _cameraShake;

    private float _nextFireTime = 0.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _laserOffsetY = 1.0f;
    [SerializeField]
    private AudioClip _laserSound;
    [SerializeField]
    private AudioClip _outOfAmmo;
    private AudioSource _audioSource;

    private bool _isShieldUp = false;
    [SerializeField]
    private GameObject _shield;
    private int _shieldStrength = 3;

    private bool _isTripleShotUp = false;
    [SerializeField]
    private GameObject _tripleShotPrefab;

    private bool _isStarBurstUp = false;
    [SerializeField]
    private GameObject _starBurstPrefab;

    [SerializeField]
    private float _powerUpTime = 5.0f;
    [SerializeField]
    private AudioClip _powerUpSound;

    [SerializeField]
    private GameObject[] _damage;
    [SerializeField]
    private GameObject _explosionPrefab;

    private Weapon _laser;
 
    void Start()
    {
        _spawnManger = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManger == null)
        {
            Debug.LogError("Player : Spawn Manager is NULL");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("Player: UIManager is NULL");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Player: AudioSource is NULL");
        }

        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        if (_cameraShake == null)
        {
            Debug.LogError("Player: CameraShake is NULL");
        }
  
        transform.position = new Vector3(0, -3.5f, 0);
        _laser = new Weapon("Laser", 0.2f, 15);

        _uiManager.SetWeaponText(_laser.name, _laser.ammoCount);

    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space)  && Time.time > _nextFireTime)
        {
            _nextFireTime = Time.time + _laser.fireRate;
            FireLaser();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            SetThrusterOn();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            SetThrusterOff();
        }

        if (_thruster < 10 && !_thrusterOn)
        {
            ThrusterCharge();
        }

        Movement();
    }

    private void SetThrusterOn()
    {
        if (_thrusterCharged)
        {
            _thruster -= _thrusterRate * Time.deltaTime;

            if (!_thrusterOn)
            {
                _speed += _thrusterBoost;
                _thrusterOn = true;
            }

            if (_thruster <= 0.0f)
            {
                _thruster = 0.0f;
                _thrusterCharged = false;
                if (_thrusterOn)
                {
                    _speed -= _thrusterBoost;
                    _thrusterOn = false;
                }
                StartCoroutine(ThrusterCharging()); // delay before re-use
            }
        }
        
        _uiManager.SetThrusterBar(_thruster);
    }

    private void ThrusterCharge()
    {
        _thruster += (_thrusterRate / 4) * Time.deltaTime;
        if (_thruster >= 10.0f)
        {
            _thruster = 10.0f;
        }

        _uiManager.SetThrusterBar(_thruster);
    }

    private void SetThrusterOff()
    {
        if (_thrusterOn)
        {
            _speed -= _thrusterBoost;
            _thrusterOn = false;
        }

    }

    private void FireLaser()
    {
        if (_laser.ammoCount > 0)
        {
            if (_isTripleShotUp)
            {
                _laserOffsetY = 0.0f;
                Vector2 laserSpawnPos = new Vector2(transform.position.x, transform.position.y + _laserOffsetY);
                Instantiate(_tripleShotPrefab, laserSpawnPos, Quaternion.identity);
            }
            else if (_isStarBurstUp)
            {
                _laserOffsetY = 0.0f;
                Vector2 laserSpawnPos = new Vector2(transform.position.x, transform.position.y + _laserOffsetY);
                Instantiate(_starBurstPrefab, laserSpawnPos, Quaternion.identity);
            }
            else
            {
                _laserOffsetY = 1.0f;
                Vector2 laserSpawnPos = new Vector2(transform.position.x, transform.position.y + _laserOffsetY);
                Instantiate(_laserPrefab, laserSpawnPos, Quaternion.identity);
            }
            _audioSource.clip = _laserSound;
            _audioSource.Play();
            _laser.ammoCount--;
            _uiManager.SetWeaponText(_laser.name, _laser.ammoCount);
        }
        else
        {
            _audioSource.clip = _outOfAmmo;
            _audioSource.Play();
        }
    }
    
    private void Movement()
    {
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(hInput, vInput, 0);
        transform.Translate(direction * _speed * Time.deltaTime);
        
        if (transform.position.x > 10.5f)
        {
            transform.position = new Vector3(-10.5f, transform.position.y, 0);
        }
        else if (transform.position.x < -10.5f)
        {
            transform.position = new Vector3(10.5f, transform.position.y, 0);
        }
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.5f, 0), 0);
    }

    public void Damage()
    {
        if (_isShieldUp)
        {
            _shieldStrength--;
            _uiManager.SetShieldsBar(_shieldStrength);
            if (_shieldStrength <= 0)
            {
                _shield.SetActive(false);
                _isShieldUp = false;
            }
        }
        else
        {
            StartCoroutine(_cameraShake.ShakeCamera(0.5f, 0.25f));  
            RemoveLife();
        }
    }

    private void RemoveLife()
    {
        _lives -= 1;

        if (_lives > 0)
        {
            _damage[_lives - 1].SetActive(true);
            _uiManager.SetLivesImage(_lives);
        }
        else if (_lives <= 0)
        {
            _lives = 0;
            _uiManager.SetLivesImage(_lives);
            GameOver();
        }
    }

    private void GameOver()
    {
        _spawnManger.StopSpawning();
        _uiManager.ShowGameOver();
        _uiManager.ShowRestartText();
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(this.gameObject, 0.25f);
    }

    public void IncrementScore(int scoreToAdd)
    {
        _score += scoreToAdd;
        _uiManager.SetScoreText(_score);
    }

    public void AddLife()
    {
        if (_lives < 3)
        {
            _audioSource.clip = _powerUpSound;
            _audioSource.Play();
            _damage[_lives - 1].SetActive(false);
            _lives += 1;
            _uiManager.SetLivesImage(_lives);
        }
    }

    public void AddAmmo()
    {
        _audioSource.clip = _powerUpSound;
        _audioSource.Play();
        _laser.ammoCount = 15;
        _uiManager.SetWeaponText("Laser", _laser.ammoCount);
    }

    public void TripleShotActive()
    {
        _audioSource.clip = _powerUpSound;
        _audioSource.Play();
        _isTripleShotUp = true;
        StartCoroutine(TripleShotTimer());
        
    }

    public void StarBurstActive()
    {
        _audioSource.clip = _powerUpSound;
        _audioSource.Play();
        _isStarBurstUp = true;
        StartCoroutine(StarBurstTimer());

    }

    public void SpeedBoostActive()
    {
        _audioSource.clip = _powerUpSound;
        _audioSource.Play();
        _speed += _speedBoost;
        StartCoroutine(SpeedBoostTimer());

    }

    public void ShieldActive()
    {
        _audioSource.clip = _powerUpSound;
        _audioSource.Play();
        _isShieldUp = true;
        if (_shieldStrength != 3)
        {
            _shieldStrength = 3;
        }
        _uiManager.SetShieldsBar(_shieldStrength);
        _shield.SetActive(true);
    }

    IEnumerator TripleShotTimer()
    {
        yield return new WaitForSeconds(_powerUpTime);
        _isTripleShotUp = false;
    }

    IEnumerator SpeedBoostTimer()
    {
        yield return new WaitForSeconds(_powerUpTime);
        _speed -= _speedBoost;
     }

    IEnumerator ThrusterCharging()
    {
        yield return new WaitForSeconds(15.0f);
        _thrusterCharged = true;
    }

    IEnumerator StarBurstTimer()
    {
        yield return new WaitForSeconds(_powerUpTime);
        _isStarBurstUp = false;
    }
}
