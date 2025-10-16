using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [Header("Tuning")]
    [SerializeField] public int damage = 1;
    [SerializeField] float speed = 3.0f;
    float disructTime = 10.0f; // projectile will be destroyed if no collisions within 5 secs

    [Header("Collision")]
    public LayerMask wallLayers;    // used to include tilemap layers for wall collisions

    [HideInInspector] public bool playerProj;

    private Rigidbody2D rigidBody;
    private Collider2D col2D;

    // to prevent collsions with object the fired it
    private Collider2D ownerCollider;
    private float ownerIgnoreTime = 0.1f; // short grace period
    private float spawnTime;

    // Helped fix bug where projectile would skip past wall
    private readonly RaycastHit2D[] hits = new RaycastHit2D[1];
    private ContactFilter2D wallFilter;
    private const float coll_buff = 0.02f; // add small buffer to collider


    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        col2D = GetComponent<Collider2D>();
        rigidBody.gravityScale = 0f;
        rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        wallFilter.useLayerMask = true;
        wallFilter.layerMask = wallLayers;
        wallFilter.useTriggers = false;
    }

    private void FixedUpdate()
    {
        Vector2 rb_velocity = rigidBody.velocity;
        // how to  
        float dist = rb_velocity.magnitude * Time.fixedDeltaTime;

        int hitCast = col2D.Cast(rb_velocity.normalized, wallFilter, hits, dist + coll_buff);

        // hitcast = number of colisions in next distance + coll_buf with walls: check if collisions occurs
        if (hitCast > 0)
        {
            Despawn();
            return;
        }
    }

    public void setDamage(int damage)
    {
        this.damage = damage;
    }

    public void Fire(Vector2 direction, bool fromPlayer, Collider2D owner = null)
    {
        playerProj = fromPlayer;

        // in player fires before moving once, shoot up
        if (direction ==  Vector2.zero)
        {
            direction = Vector2.up;
        }

        spawnTime = Time.time;

        ownerCollider = owner;
        if (ownerCollider != null)
        {
            Physics2D.IgnoreCollision(col2D, ownerCollider, true);
        }

        rigidBody.velocity = direction.normalized * speed;

        CancelInvoke();
        Invoke(nameof(Despawn), disructTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Collides with wall: destroy and exit function
        if (IsInLayerMask(other.gameObject.layer, wallLayers))
        {
            Despawn();
            return;
        }

        // Collides with an object not of type who fired it: destroy it
        // Collides with an object of type who fired it: ignore collision
        if ((playerProj && other.CompareTag("Enemy")) || (!playerProj && other.CompareTag("Player")))
        {
            // get position of projectile for kockback handeling
            Vector2 pos = other.ClosestPoint(transform.position);

            other.GetComponent<Player>()?.TakeDamage(damage, pos, true);
            other.GetComponent<EnemyBase>()?.TakeDamage(damage);
            Despawn();
            return;
            
        }

        // Prevents instant destruction on shooter object
        if (ownerCollider != null && other == ownerCollider)
        {
            // If grace time elapsed, re-enable collision
            if (Time.time - spawnTime > ownerIgnoreTime)
            {
                Physics2D.IgnoreCollision(col2D, ownerCollider, false);
            }
            return;
        }

        // ignore collisions with same class object as shooter
        if (playerProj && other.CompareTag("Player")) return;
        if (!playerProj && other.CompareTag("Enemy")) return;

    }

    // used to recognize walls/collidible tiles
    private static bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }
}
