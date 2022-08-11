using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private TMP_Text _scoreText, _endScoreText,_highScoreText;

    private int score;

    [SerializeField]
    private Animator _scoreAnimator;

    [SerializeField]
    private AnimationClip _scoreClip;
 
    [SerializeField]
    private GameObject _endPanel;

    [SerializeField]
    private Image _soundImage;

    [SerializeField]
    private Sprite _activeSoundSprite, _inactiveSoundSprite;

    public static event Action<int> UpdateScoreColor;

    [SerializeField]
    private int _totalScoreTargets;

    [HideInInspector]
    public int currentTargetIndex, currentMoveIndex;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AudioManager.Instance.AddButtonSound();

        StartCoroutine(IStartGame());        
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(Constants.DATA.MAIN_MENU_SCENE);
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToggleSound()
    {
        bool sound = (PlayerPrefs.HasKey(Constants.DATA.SETTINGS_SOUND) ? PlayerPrefs.GetInt(Constants.DATA.SETTINGS_SOUND)
            : 1) == 1;
        sound = !sound;
        PlayerPrefs.SetInt(Constants.DATA.SETTINGS_SOUND, sound ? 1 : 0);
        _soundImage.sprite = sound ? _activeSoundSprite : _inactiveSoundSprite;
        AudioManager.Instance.ToggleSound();
    }

    public void EndGame()
    {
        StartCoroutine(GameOver());
    }

    [SerializeField] private Animator _bestAnimator;
    [SerializeField] private AnimationClip _bestClip;

    private IEnumerator GameOver()
    {
        GameEnded?.Invoke();
        _scoreText.gameObject.SetActive(false);

        yield return MoveCamera(new Vector3(_cameraStartPos.x,-_cameraStartPos.y,_cameraStartPos.z));


        _endPanel.SetActive(true);
        _endScoreText.text = score.ToString();

        bool sound = (PlayerPrefs.HasKey(Constants.DATA.SETTINGS_SOUND) ?
          PlayerPrefs.GetInt(Constants.DATA.SETTINGS_SOUND) : 1) == 1;
        _soundImage.sprite = sound ? _activeSoundSprite : _inactiveSoundSprite;

        int highScore = PlayerPrefs.HasKey(Constants.DATA.HIGH_SCORE) ? PlayerPrefs.GetInt(Constants.DATA.HIGH_SCORE) : 0;
        if (score > highScore)
        {
            _highScoreText.text = "NEW BEST";
            highScore = score;
            PlayerPrefs.SetInt(Constants.DATA.HIGH_SCORE, highScore);
            _bestAnimator.Play(_bestClip.name, -1, 0f);
        }
        else
        {
            _highScoreText.text = "BEST " + highScore.ToString();
        }
    }

    public void UpdateScore(int current)
    {
        score++;
        _scoreText.text = score.ToString();
        _scoreAnimator.Play(_scoreClip.name, -1, 0f);

        //Fire UpdateScoreColorEvent
        int temp = UnityEngine.Random.Range(0, _totalScoreTargets);
        while(temp == currentTargetIndex)
        {
            temp = UnityEngine.Random.Range(0, _totalScoreTargets);
        }
        currentTargetIndex = temp;
        currentMoveIndex = current;
        UpdateScoreColor?.Invoke(currentTargetIndex);
    }

    [SerializeField]
    private Vector3 _cameraStartPos, _cameraEndPos;
    [SerializeField]
    private float _timeToMoveCamera;

    public static event Action GameStarted, GameEnded;

    private IEnumerator IStartGame()
    {
        Camera.main.transform.position = _cameraStartPos;
        _scoreText.gameObject.SetActive(false);

        //Start Event Conditons
        currentTargetIndex = 0;
        currentMoveIndex = 0;

        int temp = UnityEngine.Random.Range(0, _totalScoreTargets);
        while (temp == currentTargetIndex)
        {
            temp = UnityEngine.Random.Range(0, _totalScoreTargets);
        }
        currentTargetIndex = temp;
        UpdateScoreColor?.Invoke(currentTargetIndex);

        yield return MoveCamera(_cameraEndPos);

        _scoreText.gameObject.SetActive(true);
        score = 0;
        _scoreText.text = score.ToString();
        _scoreAnimator.Play(_scoreClip.name, -1, 0f);
        
        GameStarted?.Invoke();
    }

    private IEnumerator MoveCamera(Vector3 endPos)
    {
        var fixedUpdate = new WaitForFixedUpdate();
        Transform cameraTransform = Camera.main.transform;
        float timeElapsed = 0f;
        Vector3 startPos = cameraTransform.position;
        Vector3 offset = endPos - startPos;
        float speed = 1 / _timeToMoveCamera;
        while(timeElapsed < 1f)
        {
            timeElapsed += speed * Time.fixedDeltaTime;
            cameraTransform.position = startPos + timeElapsed * offset;
            yield return fixedUpdate;
        }

        cameraTransform.position = endPos;
    }
}
