using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;
    private float _speedBoost = 3.5f;
    private float _thrusterBoost = 2.5f;
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

        CheckThruster();

        Movement();
    }

    private void CheckThruster()
    {
        if (Input.GetKey(KeyCode.LeftShift) && _thrusterCharged)
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
        else
        {
            _thruster += (_thrusterRate / 4) * Time.deltaTime;
            if (_thruster >= 10)
            {
                _thruster = 10.0f;
            }
        }
        
        if (Input.GetKeyUp(KeyCode.LeftShift) && _thrusterOn)
        {
            _speed -= _thrusterBoost; 
            _thrusterOn = false;
        }

        _uiManager.SetThrusterBar(_thruster);
    }

    private void FireLaser()
    {
        if (_laser.ammoCount > 0)
        {
            if (_isTripleShotUp)
            {
                _laserOffsetY = 0.0f;
                Vector3 laserSpawnPos = new Vector3(transform.position.x, transform.position.y + _laserOffsetY, transform.position.z);
                Instantiate(_tripleShotPrefab, laserSpawnPos, Quaternion.identity);
            }
            else
            {
                _laserOffsetY = 1.0f;
                Vector3 laserSpawnPos = new Vector3(transform.position.x, transform.position.y + _laserOffsetY, transform.position.z);
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
            UpdateLives();
        }
    }

    private void UpdateLives()
    {
        _lives -= 1;
        _uiManager.SetLivesImage(_lives);

        if (_lives > 0)
        {
            _damage[_lives - 1].SetActive(true);
        }
        else if (_lives == 0)
        {
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

    public void TripleShotActive()
    {
        _audioSource.clip = _powerUpSound;
        _audioSource.Play();
        _isTripleShotUp = true;
        StartCoroutine(TripleShotTimer());
        
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
        _shield.SetActive(true);
    }

    IEnumerator TripleShotTimer()
    {
        while(true)
        {
            yield return new WaitForSeconds(_powerUpTime);
            _isTripleShotUp = false;
        }

    }

    IEnumerator SpeedBoostTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(_powerUpTime);
            _speed -= _speedBoost;
        }

    }

    IEnumerator ThrusterCharging()
    {
        while (true)
        {
            yield return new WaitForSeconds(15.0f);
            _thrusterCharged = true;
        }

    }
}
