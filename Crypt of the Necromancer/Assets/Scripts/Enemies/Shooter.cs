using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : EnemyBase
{
    [Header("Movement")]
    [SerializeField] private float changeDirChance = 0.1f;
    private Vector2 currentCardinal = Vector2.zero;
    private SpriteRenderer spriteRenderer; // for animation

    [Header("Proj")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float shootRange = 6f;
    [SerializeField] private float coolDown = 1f; //cooldown for firing 
    [SerializeField] private int projectileDamage = 2;
    [SerializeField] private bool requireClearLOS = true; // true = enemy need line of sight (for testing to see which feels better)

    private float nextShootTime;    // used with cooldown


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        currentCardinal = RandomCardinalDir();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Gets random cardinal direction 
    protected override Vector2 GetDirection()
    {
        // random walking (same as walker)
        if (Random.value < changeDirChance || currentCardinal == Vector2.zero)
        {
            currentCardinal = RandomCardinalDir();
        }

        // asset only has right walking, need to flip when moving left 
        if (spriteRenderer)
        {
            if (currentCardinal == Vector2.right)
                spriteRenderer.flipX = false; // facing right (default)
            else if (currentCardinal == Vector2.left)
                spriteRenderer.flipX = true;  // facing left
        }

        // Shoot at player when they enter range trigger circle2d collider
        Shoot();

        return currentCardinal;
    }

    // Handels how walker deals with running into walls
    protected override void OnBlocked()
    {
        Vector2 origin = col.bounds.center; // gets center of walker's collider for refernce

        // Try to get the wall blocking us and nudge off it
        RaycastHit2D hit = Physics2D.Raycast(origin, currentCardinal, wallCheckDistance, wallLayers);
        if (hit.collider)
        {
            transform.position += (Vector3)(hit.normal * 0.03f); // nudge off wall
        }
        else
        {
            // nudge backwards if we didn't catch a collider with raycast/already inside collider
            transform.position += (Vector3)(-currentCardinal * 0.03f);
        }

        // Reverse direction, or pick a new one if the opposite is also blocked (narrow hallway)
        Vector2 reverse = -currentCardinal;
        bool blockedOpposite = Physics2D.Raycast(origin, reverse, wallCheckDistance, wallLayers);
        currentCardinal = blockedOpposite ? RandomCardinalDir() : reverse;

        // Safety: ensure we have a direction
        if (currentCardinal == Vector2.zero) currentCardinal = RandomCardinalDir();
    }

    // try to hit player if in range collider
    private void Shoot()
    {
        // prevent runtime exception on player death
        if (!player) return;

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
