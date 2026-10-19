using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialUI : MonoBehaviour
{
    public static TutorialUI Main;

    [SerializeField]
    private GameObject walkTutorial;
    [SerializeField]
    private GameObject combatTutorial;
    [SerializeField]
    private GameObject ultimateTutorial;
    [SerializeField]
    private GameObject keyTutorial;

    private bool isTutorial = false;

    public void Awake()
    {
        Main = this;
        Debug.Log(SceneManager.GetActiveScene().name);
        isTutorial = SceneManager.GetActiveScene().name == "Tutorial";
    }


    void Start()
    {
        disableAll();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void disableAll()
    {
        walkTutorial.SetActive(false);
        combatTutorial.SetActive(false);
        ultimateTutorial.SetActive(false);
        keyTutorial.SetActive(false);
    }

    public void ShowWalkTutorial()
    {
        if (!isTutorial)
        {
            return;
        }
        disableAll();
        walkTutorial.SetActive(true);
    }

    public void ShowCombatTutorial()
    {
        if (!isTutorial)
        {
            return;
        }
        disableAll();
        combatTutorial.SetActive(true);
    }

    public void ShowUltimateTutorial()
    {
        if (!isTutorial)
        {
            return;
        }
        disableAll();
        ultimateTutorial.SetActive(true);
    }

    public void ShowKeyTutorial()
    {
        if (!isTutorial)
        {
            return;
        }
        disableAll();
        keyTutorial.SetActive(true);
        isTutorial = false;
    }

}
