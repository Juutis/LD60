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
        if (attackAction.WasPressedThisFrame() && inputBuffer.Count <= 3)
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
                movement.SlowMoving(true);
                battleAnim.PlayState(currentAttack.AnimState, true);
            }
            else
            {
                movement.SlowMoving(false);
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
        Vector3 hitCastDir = movement.GetDirection();
        Vector3 hitCastStart = transform.position + 0.5f * Vector3.up + hitCastDir * 0.5f;
        float sphereRadius = 0.5f;

        // Physics.Raycast(hitCastStart, hitCastDir, out RaycastHit hit, 4f, LayerMask.GetMask("Enemy"));
        Collider[] cols = Physics.OverlapSphere(hitCastStart, sphereRadius, LayerMask.GetMask("Enemy"));
        Debug.DrawLine(hitCastStart + Vector3.up, hitCastStart - Vector3.up, Color.red, 5f);
        Debug.DrawLine(hitCastStart + Vector3.up + Vector3.forward * 0.2f, hitCastStart + Vector3.up - Vector3.forward * 0.2f, Color.red, 5f);
        Debug.DrawLine(hitCastStart + Vector3.up + Vector3.right * 0.2f, hitCastStart + Vector3.up - Vector3.right * 0.2f, Color.red, 5f);

        foreach (Collider col in cols)
        {
            if (col != null && col.TryGetComponent(out Health enemyHealth))
            {
                Debug.Log("Hit!");
                enemyHealth.TakeDamage(1);
            }
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
