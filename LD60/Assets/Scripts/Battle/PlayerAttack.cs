using JetBrains.Annotations;
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
    [SerializeField]
    private float ultChargeDuration;
    [SerializeField]
    private Transform playerModel;
    [SerializeField]
    private ParticleSystem ultParticles;

    [SerializeField]
    private AudioClip[] hitSounds;

    private AudioSource audioSource;

    private InputAction attackAction;
    private InputAction ultAction;
    private PlayerMovement movement;

    private Attack currentAttack;
    private float actionEnded;

    private List<AttackType> currentCombo = new();

    private BattleAnimator battleAnim;

    private bool isAttacking = false;

    private bool isUlting = true;
    private float ultStarted;
    private int ultCharges = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        attackAction = InputSystem.actions.FindAction("attack");
        ultAction = InputSystem.actions.FindAction("ult");
        battleAnim = GetComponentInChildren<BattleAnimator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        BattleUIManager.instance.UpdateBatteryCounter(ultCharges);

        if (isUlting)
        {
            float coef = Mathf.Lerp(0f, 3f, (Time.time - ultStarted)*2f % 1f);
            Vector3 uPos = playerModel.position;
            Vector3 orig = uPos + Vector3.up * 0.5f;
            Vector3 uDir = playerModel.position + playerModel.forward * coef + Vector3.up * 0.5f;

            List<Tuple<Vector3, Vector3>> rays = new() {
                new(orig, uDir),
                new(orig, uDir + playerModel.right * coef * 0.5f),
                new(orig, uDir - playerModel.right * coef * 0.5f),
                new(orig, uDir + playerModel.right * coef * 0.25f),
                new(orig, uDir - playerModel.right * coef * 0.25f)
            };

            foreach(Tuple<Vector3, Vector3> ray in rays)
            {
                Vector3 origin = ray.Item1;
                Vector3 end = ray.Item2;
                Vector3 dir = (end - origin).normalized;
                float distance = (end - origin).magnitude;
                Debug.DrawLine(origin, end, Color.violetRed);
                Physics.Raycast(origin, dir, out RaycastHit hitInfo, distance, LayerMask.GetMask("Enemy"));

                if (hitInfo.collider != null && hitInfo.collider.gameObject.TryGetComponent(out Health enemy))
                {
                    enemy.TakeDamage(1000);
                }
            }

            if (Time.time - ultStarted > ultChargeDuration)
            {
                isUlting = false;
                ultParticles.Stop();
            }
            return;
        }

        if (ultAction.WasPressedThisFrame() && !isAttacking && ultCharges > 0)
        {
            isUlting = true;
            ultStarted = Time.time;
            ultCharges--;
            ultParticles.Play();
        }
        else if (attackAction.WasPressedThisFrame() && !isAttacking)
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

    public void AddUltCharge(int num = 1)
    {
        ultCharges += num;
    }

    public void AttackHit()
    {
        Vector3 hitCastDir = movement.GetDirection();
        Vector3 hitCastStart = transform.position + 0.5f * Vector3.up + hitCastDir * 0.5f;
        float sphereRadius = 0.5f;
        if (currentAttack.name == "Finisher")
        {
            hitCastStart = transform.position + 0.5f * Vector3.up;
            sphereRadius = 1.0f;
        }

        // Physics.Raycast(hitCastStart, hitCastDir, out RaycastHit hit, 4f, LayerMask.GetMask("Enemy"));
        Collider[] cols = Physics.OverlapSphere(hitCastStart, sphereRadius, LayerMask.GetMask("Enemy"));
        Debug.DrawLine(hitCastStart + Vector3.up, hitCastStart - Vector3.up, Color.red, 5f);
        Debug.DrawLine(hitCastStart + Vector3.up + Vector3.forward * 0.2f, hitCastStart + Vector3.up - Vector3.forward * 0.2f, Color.red, 5f);
        Debug.DrawLine(hitCastStart + Vector3.up + Vector3.right * 0.2f, hitCastStart + Vector3.up - Vector3.right * 0.2f, Color.red, 5f);

        var hits = 0;
        foreach (Collider col in cols)
        {
            if (col != null && col.TryGetComponent(out Health enemyHealth))
            {
                enemyHealth.TakeDamage(currentAttack.damage);
                hits++;
            }
        }

        if (hits > 0)
        {
            var clip = hitSounds[UnityEngine.Random.Range(0, hitSounds.Length)];
            audioSource.PlayOneShot(clip);
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
