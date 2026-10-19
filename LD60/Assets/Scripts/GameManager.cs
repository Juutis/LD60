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
    private int batteryCount = 0;
    private int musicsCount = 0;

    void Awake()
    {
        if (main == null) {
            main = this;
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

    public void IncreaseMusics()
    {
        musicsCount++;
    }

    public void IncreaseBatteries()
    {
        batteryCount++;
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
        afterLevelCleared = null;
        SceneManager.LoadScene("Death");
    }


}
