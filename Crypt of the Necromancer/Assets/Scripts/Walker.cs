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

        // flip horizontally by changing the object's localScale
        if (Mathf.Abs(currentCardinal.x) > 0.001f)
        {
            Vector3 scale = transform.localScale;
            float baseX = Mathf.Abs(scale.x);
            scale.x = baseX * (currentCardinal.x > 0 ? -1f : 1f);
            transform.localScale = scale;
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
