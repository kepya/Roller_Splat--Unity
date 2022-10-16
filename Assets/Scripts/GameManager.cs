using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    public TextMeshProUGUI time;
    public GameObject restartObject;
    public AudioClip gameOverClip;
    private AudioSource audioSource;

    private GroundPiece[] groundPieces;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SetUpNewLevel();
    }

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        } else if (singleton != null)
        {
            Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnLevelFinishLoading(Scene scene, LoadSceneMode mode)
    {
        SetUpNewLevel();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishLoading;
    }

    public void checkCompleted()
    {
        bool isFinish = true;

        for (int i = 0; i < groundPieces.Length; i++)
        {
            if(groundPieces[i].isColored == false)
            {
                isFinish = false;
                break;
            }
        }

        if (isFinish)
        {
            Debug.Log("next");
            nextLevel();
        }
    }

    public void nextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUpNewLevel()
    {
        groundPieces = GameObject.FindObjectsOfType<GroundPiece>();

        if (restartObject)
        {
            restartObject.gameObject.SetActive(false);
        }
        if (time)
        {
            float timeValue = 20;
            timeValue = (timeValue + (SceneManager.GetActiveScene().buildIndex * 10));
            time.text = "Time: " + timeValue;
            StartCoroutine(StartCount(timeValue));
        }
    }

    public void QuitQame()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitQame()
    {
        #if UNITY_EDITOR
             UnityEditor.EditorApplication.isPlaying = false;
        #else
             Application.Quit();
        #endif
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator StartCount(float timeValue)
    {
       while(timeValue > 0)
        {
            yield return new WaitForSeconds(1f);
            timeValue--;
            time.text = "Time: " + timeValue;

            if (timeValue == 0)
            {
                restartObject.gameObject.SetActive(true);
                if (audioSource && gameOverClip)
                {
                    audioSource.PlayOneShot(gameOverClip);
                }
            }
        }
    }
}
