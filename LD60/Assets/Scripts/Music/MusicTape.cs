using UnityEngine;

[CreateAssetMenu(fileName = "MusicTape", menuName = "Scriptable Objects/MusicTape")]
public class MusicTape : ScriptableObject
{
    [SerializeField]
    private AudioClip audio;
    public AudioClip Audio { get { return audio; } }

    [SerializeField]
    private  float volume;
    public float Volume { get { return volume; } }


    [SerializeField]
    private string songName;
    public string SongName { get { return songName; } }



    [SerializeField]
    private Sprite sprite;
    public Sprite Sprite { get { return sprite; } }
}
