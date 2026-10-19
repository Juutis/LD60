using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager main;

    [SerializeField]
    private Transform worldMapContainer;

    void Awake()
    {
        if (main == null) {
            main = this;
        }
    }

    public void LoadLevel(LevelConfig level)
    {
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

    public void LoadWorldMap()
    {
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


}
