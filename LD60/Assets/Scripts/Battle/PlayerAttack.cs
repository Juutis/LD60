using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer handRenderer;
    [SerializeField]
    private List<Attack> attacks;
    [SerializeField]
    private float comboTimer = 0.5f;

    private InputAction attackAction;
    private PlayerMovement movement;

    private Attack currentAttack;
    private float actionStarted;
    private float actionEnded;

    private List<AttackType> currentCombo = new();

    private List<AttackType> inputBuffer = new();

    private BattleAnimator battleAnim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        attackAction = InputSystem.actions.FindAction("attack");
        battleAnim = GetComponentInChildren<BattleAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (attackAction.WasPressedThisFrame())
        {
            inputBuffer.Add(AttackType.normal);
        }

        if (currentAttack != null)
        {
            float cooldownOver = currentAttack.attackTime + currentAttack.cooldown;

            if (Time.time - actionStarted > cooldownOver)
            {
                currentAttack = null;
                actionEnded = Time.time;
            }
            else if (Time.time - actionStarted > currentAttack.attackTime)
            {
                // attack
            }
        }

        if (currentAttack == null)
        {
            if (Time.time - actionEnded > comboTimer)
            {
                currentCombo = new();
            }

            if (inputBuffer.Count > 0)
            {
                AttackType next = inputBuffer.First();
                inputBuffer.RemoveAt(0);
                currentCombo.Add(next);
                Attack initialAttack = attacks.FirstOrDefault(x => x.type == next && x.isInitial);
                Attack nextAttack = currentCombo.Count <= 1 ?
                    initialAttack :
                    attacks.FirstOrDefault(x => isEqualCombo(currentCombo, x.comboPattern));

                if (initialAttack == null)
                {
                    Debug.LogError($"No initial attack defined for attack type {next}!");
                }
                else if (nextAttack == null) // combo finished, start a new one
                {
                    nextAttack = initialAttack;
                    currentCombo = new();
                }

                currentAttack = nextAttack;
                actionStarted = Time.time;
                movement.BlockMoving(true);
                battleAnim.PlayState(currentAttack.AnimState);
            }
            else
            {
                movement.BlockMoving(false);
            }
        }
    }

    private bool isEqualCombo(List<AttackType> combo1, List<AttackType> combo2)
    {
        if (combo1.Count != combo2.Count)
        {
            return false;
        }

        for (int i = 0; i < combo1.Count; i++)
        {
            if (combo1[i] != combo2[i])
            {
                return false;
            }
        }

        return true;
    }

    public void AttackHit()
    {
        Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 2f, LayerMask.NameToLayer("Enemy"));
        if (hit.collider != null && hit.collider.TryGetComponent(out Health enemyHealth))
        {
            enemyHealth.TakeDamage(1);
        }
    }

    public void AttackOver()
    {

    }
}

[Serializable]
public class Attack
{
    public string name;
    public AttackType type;
    public float attackTime;
    public float damage;
    public float cooldown;
    public List<AttackType> comboPattern;
    public bool isInitial;
    public string AnimState;
}

public enum AttackType
{
    normal,
    heavy,
    ult
}
