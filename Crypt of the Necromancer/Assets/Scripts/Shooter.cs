using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : EnemyBase
{
    [Header("Movement")]
    [SerializeField] private float changeDirChance = 0.1f;
    private Vector2 currentCardinal = Vector2.zero;

    [Header("Proj")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float shootRange = 6f;
    [SerializeField] private float coolDown = 1f; //cooldown for firing 
    [SerializeField] private int projectileDamage = 2;
    [SerializeField] private bool requireClearLOS = true; // true = enemy need line of sight (for testing to see which feels better)

    private float nextShootTime;    // used with cooldown

    //protected override void Awake()
    //{
    //    base.Awake();
    //}

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        currentCardinal = RandomCardinalDir();
    }

    // Update is called once per frame
    protected override Vector2 GetDirection()
    {
        // random walking (same as walker)
        if (Random.value < changeDirChance || currentCardinal == Vector2.zero)
        {
            currentCardinal = RandomCardinalDir();
        }

        // Shoot at player when they enter range trigger circle2d collider
        Shoot();

        return currentCardinal;
    }

    protected override void OnBlocked()
    {
        Vector2 reverse = -currentCardinal;
        // check if reverse direction also is blocked (narrow hallway or sign/chest next to wall)
        bool blockedOpposite = Physics2D.Raycast(transform.position, reverse, wallCheckDistance, wallLayers);
        if (blockedOpposite)
        {
            currentCardinal = RandomCardinalDir();
        }
        currentCardinal = reverse;
    }

    // try to hit player if in range collider
    private void Shoot()
    {
        Vector2 dir = (player.position - transform.position);
        float dist = dir.magnitude; // length from player

        if (dist > shootRange) return;

        // prevent shots if wall is between enemy and player (but in range of enemy attack)
        // AKA enemy can't "see" player
        if (requireClearLOS)
        {
            if (Physics2D.Raycast(transform.position, dir.normalized, dist, wallLayers)) return;
        }

        if (Time.time < nextShootTime) return; // still in cooldown

        // use cooldown to set next possible shoot time
        nextShootTime = Time.time + coolDown;

        GameObject go = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile proj = go.GetComponent<Projectile>();

        if (proj)// safegaurd in case I forget to add or accidently remove projectile
        {
            proj.damage = projectileDamage;
            proj.Fire(dir.normalized, false, col);
        }
    }
}
