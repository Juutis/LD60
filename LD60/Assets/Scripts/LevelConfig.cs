using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Scriptable Objects/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [SerializeField]
    private string levelName = "level";
    public string LevelName { get { return levelName; } }

    [HideInInspector]
    public string scenePath;

    [SerializeField]
    private LevelType levelType;
    public LevelType LevelType {get {return levelType;}}
    public float EncounterChance = 1.0f;
}

public enum LevelType
{
    None,
    RandomEncounter,
    PointOfInterest
}
