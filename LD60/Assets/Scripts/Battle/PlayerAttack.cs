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
    private float actionEnded;

    private List<AttackType> currentCombo = new();

    private BattleAnimator battleAnim;

    private bool isAttacking = false;

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
        if (attackAction.WasPressedThisFrame() && !isAttacking)
        {
            AttackType nextType = AttackType.normal;
            currentCombo.Add(nextType);
            Attack nextAttack;

            if (currentCombo.Count == 1)
            {
                nextAttack = attacks.FirstOrDefault(x => x.type == nextType && x.isInitial);
            }
            else
            {
                nextAttack = attacks.FirstOrDefault(x => isEqualCombo(currentCombo, x.comboPattern));
            }

            if (nextAttack == null) // combo finished, start a new one
            {
                nextAttack = attacks.FirstOrDefault(x => x.type == nextType && x.isInitial);
                currentCombo = new() { nextType };
            }

            currentAttack = nextAttack;
            movement.SlowMoving(true);
            battleAnim.PlayState(currentAttack.AnimState, true);
            isAttacking = true;
        }

        if (!isAttacking)
        {
            movement.SlowMoving(false);
        }
        else
        {
            movement.SlowMoving(true);
        }

        if (!isAttacking && (Time.time - actionEnded) > comboTimer)
        {
            currentCombo = new();
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
                enemyHealth.TakeDamage(currentAttack.damage);
            }
        }
    }

    public void AttackOver()
    {
        isAttacking = false;
        actionEnded = Time.time;
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
