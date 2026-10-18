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

    private string currentState;
    private bool lockAnimation = false;
    public bool isDead = false;

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
        if (isDead)
        {
            anim.Play("Die");
        }
    }

    public void PlayState(string stateName, bool isAttackState = false)
    {
        if (lockAnimation || isDead)
        {
            return;
        }
        if (stateName != currentState)
        {
            anim.Play(stateName);
            currentState = stateName;
            if (isAttackState)
            {
                lockAnimation = true;
            }
        }
    }

    public void Die()
    {
        isDead = true;
    }

    public void applyDamageCallback()
    {
        applyDamageEvent.Invoke();
    }

    public void attackFinishedCallback()
    {
        attackFinishedEvent.Invoke();
        lockAnimation = false;
    }
}
