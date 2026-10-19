using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager main;

    [SerializeField]
    private Transform worldMapContainer;

    private UnityAction afterLevelCleared;

    private bool tutorial = true;

    public bool Tutorial {get {return tutorial;}}
    private int batteryCount = 0;
    private int musicsCount = 0;

    void Awake()
    {
        if (main == null) {
            main = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "worldmap")
        {
            if (worldMapContainer != null && !worldMapContainer.gameObject.activeSelf)
            {
                worldMapContainer.gameObject.SetActive(true);
            }
        }
    }

    public void LoadLevel(LevelConfig level, UnityAction afterLevelCleared)
    {
        this.afterLevelCleared = afterLevelCleared;
        Time.timeScale = 0f;
        worldMapContainer.gameObject.SetActive(false);
        TransitionManager.main.Transition(TransitionType.FadeOut, delegate
            {
                SceneManager.LoadScene(level.scenePath);
                TransitionManager.main.Transition(TransitionType.FadeIn, delegate
                    {
                        Time.timeScale = 1f;
                    }
                );
            }
        );
    }

    public void ReloadAfterDeath()
    {
        if (tutorial)
        {
            SceneManager.LoadScene("Tutorial");
        } else
        {
            LoadWorldMap();
        }
    }

    public void PassLevel()
    {
        musicsCount++;
        MusicManager.main.PassLevel();
    }

    public void IncreaseBatteries()
    {
        batteryCount++;
    }

    public void UseBatteries()
    {
        batteryCount--;
    }

    public int GetBatteries()
    {
        return batteryCount;
    }

    public int GetMusics()
    {
        return musicsCount;
    }

    public void LoadWorldMap()
    {
        if (tutorial)
        {
            tutorial = false;
        }
        if (afterLevelCleared != null)
        {
            afterLevelCleared();
            afterLevelCleared = null;
        }
        Time.timeScale = 0f;
        TransitionManager.main.Transition(TransitionType.FadeOut, delegate
            {
                SceneManager.LoadScene("worldmap");
                worldMapContainer.gameObject.SetActive(true);
                TransitionManager.main.Transition(TransitionType.FadeIn, delegate
                    {
                        Time.timeScale = 1f;
                    }
                );
            }
        );
    }

    public void Die()
    {
        MusicManager.main.Die();
        afterLevelCleared = null;
        SceneManager.LoadScene("Death");
    }


}
