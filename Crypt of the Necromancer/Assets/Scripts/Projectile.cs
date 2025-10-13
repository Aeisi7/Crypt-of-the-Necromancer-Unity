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


    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        col2D = GetComponent<Collider2D>();
        rigidBody.gravityScale = 0f;
        rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
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
