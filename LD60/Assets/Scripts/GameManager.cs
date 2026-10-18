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
        main = this;
    }

    public void LoadLevel(LevelConfig level)
    {
        worldMapContainer.gameObject.SetActive(false);
        SceneManager.LoadScene(level.scenePath);
    }

    public void LoadWorldMap()
    {
        SceneManager.LoadScene("worldmap");
        worldMapContainer.gameObject.SetActive(true);
    }


}
