using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomEncounterManager : MonoBehaviour
{

    private bool started = false;
    private float movedDistanceSinceEncounter = 0f;

    private float movedDistanceSinceRandomize = 0f;

    [SerializeField]
    private float encounterTreshold = 50f;

    private int encounterCount = 0;

    private float distanceInterval = 0.1f;

    [SerializeField]
    private List<LevelConfig> encounters;

    private WorldMapPlayerCharacter playerCharacter;
    void Start()
    {
    }

    void Update()
    {
        if (playerCharacter == null)
        {
            playerCharacter = FindAnyObjectByType<WorldMapPlayerCharacter>();
            return;
        }
        if (!started)
        {
            started = true;
            movedDistanceSinceRandomize = playerCharacter.MovedDistance;
            movedDistanceSinceEncounter = playerCharacter.MovedDistance;
            return;
        }

        if ((playerCharacter.MovedDistance - movedDistanceSinceRandomize) > distanceInterval)
        {
            //Debug.Log($"Player has moved ({playerCharacter.MovedDistance - movedDistanceSinceRandomize}) > {distanceInterval}");
            movedDistanceSinceRandomize = playerCharacter.MovedDistance;
            RandomizeEncounter();
        }
    }

    public void RandomizeEncounter()
    {
        float playerMovementDelta = playerCharacter.MovedDistance - movedDistanceSinceEncounter;

        float chance = Random.Range(0, playerMovementDelta);
        //Debug.Log($"Randomize movedDistanceSinceEncounter:[{movedDistanceSinceEncounter}] playerMovementDelta:[{playerMovementDelta}] chance:[{chance}]");
        if (chance > encounterTreshold)
        {
            encounterCount += 1;
            Debug.Log($"Start random encounter! Count: {encounterCount} MovedDist: [{playerMovementDelta}] chance: [{chance}]");
            movedDistanceSinceEncounter = playerCharacter.MovedDistance;
            StartRandomEncounter();
        } else
        {
            //Debug.Log($"It didn't happen! Chance: [{chance}] delta: [{playerMovementDelta}]");
        }
    }

    private void StartRandomEncounter()
    {
        var sumOfChances = encounters.Sum(it => it.EncounterChance);
        var target = Random.Range(0, sumOfChances);
        float accumulator = 0.0f;
        LevelConfig levelConfig = null;
        for(var i = 0; i < encounters.Count; i++)
        {
            accumulator += encounters[i].EncounterChance;
            if (accumulator > target)
            {
                levelConfig = encounters[i];
                break;
            }
        }
        if (levelConfig != null)
        {
            GameManager.main.LoadLevel(levelConfig, delegate
                    {
                    });
        }
    }

}
