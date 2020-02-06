using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private GameObject _gameOverText;
    [SerializeField]
    private GameObject _restartText;

    [SerializeField]
    private Image _lives;
    [SerializeField]
    private Sprite[] _livesSprites;

    [SerializeField]
    private GameManager _gameManager;

    private float _flickerDelay = 0.5f;

    // Start is called before the first frame update
    private void Start()
    {
        _gameOverText.SetActive(false);
        _scoreText.text = "Score : 00";
        _restartText.SetActive(false);
    }

    public void SetScoreText(int playerScore)
    {
         _scoreText.text = "Score : " + playerScore;
    }

    public void SetLivesImage(int currLives)
    {
        _lives.sprite = _livesSprites[currLives];
    }

    public void ShowGameOver()
    {
        _gameManager.SetGameOver(true);
        _gameOverText.SetActive(true);
        StartCoroutine(FlickerDelay());
    }

    public void ShowRestartText()
    {
        _restartText.SetActive(true);
    }

    IEnumerator FlickerDelay()
    {
        while(true)
        {
            yield return new WaitForSeconds(_flickerDelay);
            if (_gameOverText.GetComponent<Text>().color == Color.red)
            {
                _gameOverText.GetComponent<Text>().color = Color.yellow;
            }
            else
            {
                _gameOverText.GetComponent<Text>().color = Color.red;
            }
        }
    }
}
