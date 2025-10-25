using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Walker : EnemyBase
{
    [SerializeField] private int damage = 4;
    public float changeDirChance = 0.1f;
    private Vector2 currentCardinal = Vector2.zero;
    private SpriteRenderer spriteRenderer; // for animation

    protected override void Awake()
    {
        base.Awake();
        // walker has highest collison damage, as its the only way it deals damge
        colDamage = damage;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        currentCardinal = RandomCardinalDir();
    }

    // Gets random cardinal direction 
    protected override Vector2 GetDirection()
    {
        // handels both change scenarios (movement stopped or 
        if (Random.value < changeDirChance || currentCardinal == Vector2.zero)
        {
            currentCardinal = RandomCardinalDir();
        }

        // flip horizontally by changing the object's localScale
        if (spriteRenderer)
        {
            if (currentCardinal == Vector2.right)
                spriteRenderer.flipX = true; // facing right 
            else if (currentCardinal == Vector2.left)
                spriteRenderer.flipX = false;  // facing left (default)
        }


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

}
