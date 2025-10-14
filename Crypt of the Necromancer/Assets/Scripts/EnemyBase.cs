using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class EnemyBase : MonoBehaviour
{
    // --------- This will be for capping number of enemies spawned by type --------
    // create dictionary to map the type of enemy to the count of that enemy in a scene.
    private static readonly Dictionary<Type, int> Counts = new();
    
    // lambda function to track total number of enemies
    public static int GetCountAll() => Counts.Values.Sum();

    // Generic and non-generic method to get counts of subclasses
    public static int GetCount<T>() where T : EnemyBase =>
        Counts.TryGetValue(typeof(T), out var c) ? c : 0;
    public static int GetCountByType(Type t) =>
        Counts.TryGetValue(t, out var c) ? c : 0;


    [Header("Common")]
    [SerializeField] protected float moveSpeed = 1f;
    [SerializeField] protected int colDamage = 1;   // damage enemy deals when coliding with player
    [SerializeField] protected LayerMask wallLayers;    // for detecting walls
    [SerializeField] protected int health = 4;

    [Header("Wall avoidance Params")]
    [SerializeField] protected float wallCheckDistance = 0.6f;  // distance to wall where enemy reverses direction
    [SerializeField] protected float recheckInterval = 0.1f;    // how often to chekc if near wall

    // Gameobject components
    protected Rigidbody2D rbdy;
    protected Collider2D col;
    Vector2 moveDir = Vector2.zero;
    
    protected Transform player;        // for tracking player location

    private float tickAccum; // for TickMove call in FixedUpdate

    // -------- Functions for tracking number  of enemies spawned ---------------
    protected virtual void OnEnable()
    {
        // Get type of enemy, then increment count of that type in dictionary
        Type t = GetType();
        // uses conditional operator in case of first enemy to prevent key not found exception
        Counts[t] = Counts.TryGetValue(t, out var c) ? c + 1 : 1; 
    }

    protected virtual void OnDisable()
    {
        Type t = GetType();
        if (Counts.TryGetValue(t, out var c))
        {
            c = Mathf.Max(0, c - 1);
            Counts[t] = c;
        }
    }
    // --------------------------------------------------------------------------


    protected virtual void Awake()
    {
        // on awake define and set up components
        rbdy = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rbdy.gravityScale = 0f;
        rbdy.isKinematic = true;
        col.isTrigger = true;

        // get player transform
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if(p != null ) player = p.transform;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        InvokeRepeating(nameof(TickMove), 0f, recheckInterval);
    }

    protected virtual void FixedUpdate()
    {
        tickAccum += Time.fixedDeltaTime;
        if (tickAccum >= recheckInterval)
        {
            tickAccum = 0f;
            TickMove();
        }
    }

    // want subclasses to set desired direction due to different disired AI
    protected abstract Vector2 GetDirection();

    protected virtual void TickMove()
    {
        moveDir = GetDirection();
        if (moveDir == Vector2.zero) return;

        float step = moveSpeed * recheckInterval;
        const float buffer = 0.05f; // buffer to handel turn before wall collison

        //filter to check wall collisions only, ignore triggers
        ContactFilter2D filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = wallLayers,
            useTriggers = true
        };

        Collider2D[] overlaps = new Collider2D[1]; // needs to be array for use in OverLapCollider()

        // if last step caused wall overlap, block it
        if (col.OverlapCollider(filter, overlaps) > 0)
        {
            OnBlocked();
            return;
        }

        // Cast the enemy collider along the intended step
        var hits = new RaycastHit2D[1];
        int hitCount = col.Cast(moveDir, filter, hits, step + buffer);

        if (hitCount > 0)
        {
            // We’re about to hit a wall — ask subclass to change direction.
            OnBlocked();
            return;
        }

            Vector2 newPos = (Vector2)transform.position + moveDir * moveSpeed * recheckInterval;
        rbdy.MovePosition(newPos);
    }

    protected virtual void OnBlocked() { }

    // Deal damage on collison with players (overidable in subclasses)
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Player player = other.GetComponent<Player>();

        // get position of projectile for kockback handeling
        Vector2 pos = other.ClosestPoint(transform.position);

        player.TakeDamage(colDamage, pos);
    }

    // for walking in cardinal directions (2 enemies will use)
    protected static readonly Vector2[] Cardinals = new[]
    {
        Vector2.up, Vector2.down, Vector2.left, Vector2.right
    };

    protected Vector2 RandomCardinalDir()
    {
        return Cardinals[UnityEngine.Random.Range(0, Cardinals.Length)];
    }

    public virtual void TakeDamage(int damage)
    {
        if ((health - damage) > 0)
        {
            health -= damage;
            return;
        }
        // character died
        Invoke(nameof(Despawn), 0.01f);
    }

    protected virtual void Despawn()
    {
        Destroy(gameObject);
    }
}
