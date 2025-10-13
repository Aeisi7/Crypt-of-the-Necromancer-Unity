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

    protected override Vector2 GetDirection()
    {
        // handels both change scenarios (movement stopped or 
        if (Random.value < changeDirChance || currentCardinal == Vector2.zero)
        {
            currentCardinal = RandomCardinalDir();
        }

        // asset only has left walking, need to flip when moving right 
        if (spriteRenderer) 
        {
            if (currentCardinal == Vector2.left)
                spriteRenderer.flipX = false; // facing left (default)
            else if (currentCardinal == Vector2.right)
                spriteRenderer.flipX = true;  // facing right
        }

        return currentCardinal;
    }

    protected override void OnBlocked()
    {
        // Try the opposite first
        Vector2 reverse = -currentCardinal;

        // If opposite direction also blocked, pick a new random valid one
        bool blockedOpposite = Physics2D.Raycast(transform.position, reverse, wallCheckDistance, wallLayers);
        currentCardinal = blockedOpposite ? RandomCardinalDir() : reverse;
    }

}
