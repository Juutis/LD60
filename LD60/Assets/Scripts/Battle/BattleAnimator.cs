using UnityEngine;
using UnityEngine.Events;

public class BattleAnimator : MonoBehaviour
{
    private Animator anim;

    [SerializeField]
    private UnityEvent applyDamageEvent;

    [SerializeField]
    private UnityEvent attackFinishedEvent;

    public string DebugState;

    public bool TriggerDebug;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TriggerDebug)
        {
            PlayState(DebugState);
            TriggerDebug = false;
        }
    }

    public void PlayState(string stateName)
    {
        anim.Play(stateName);
    }

    public void applyDamageCallback()
    {
        applyDamageEvent.Invoke();
    }

    public void attackFinishedCallback()
    {
        attackFinishedEvent.Invoke();
    }
}
