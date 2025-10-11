using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    
    public GameObject projectilePrefab;
    public Transform firepoint;

    // Add serializable feilds for testing
    [Header("Testing")]
    [SerializeField] private float mSpeed = 0.3f;   // player movement speed
    [SerializeField] private int maxHealth = 12;
    [SerializeField] private int curHealth = 12;
    [SerializeField] private int chestKeys = 0;
    [SerializeField] private int projDamage = 1; // default damage
    [SerializeField] private int maxMana = 20;
    [SerializeField] private int curMana = 20;
    [SerializeField] private float manaRechargeSpeed = 2.0f;
    [SerializeField] private bool levelKey = false;

    [Header("Knockback")]
    [SerializeField] private float knockBackSpeed = 6f;
    [SerializeField] private float knockBackDuration = 0.25f;
    [SerializeField] private float invincibleDuration = 0.5f;

    // knockback vars
    private bool isKnockBack = false;
    private Vector2 knockBackVel;
    private float knockBackUntil = 0f; // will point of time that knockback last until (will use Time.time)

    private bool invincible = false;
    private float invincibleUntil = 0f;

    [Header("Damage Effects")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashSpeed = 14f; 

    private SpriteRenderer pSpriteRen;
    private Rigidbody2D rigidBody;
    private Vector2 mInput;     // movement input
    private Vector2 lastMoveDir = Vector2.up; // default shoot direction is up

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>(); // initializes rigidBody on start
        pSpriteRen = GetComponent<SpriteRenderer>();
        if (pSpriteRen == null) pSpriteRen = GetComponentInChildren<SpriteRenderer>(); // handles if a child sprite
        InvokeRepeating(nameof(ManaRecovery), manaRechargeSpeed, manaRechargeSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        // read input only if not in knockback
        if (!isKnockBack)
        {
            mInput.x = Input.GetAxisRaw("Horizontal");
            mInput.y = Input.GetAxisRaw("Vertical");

            if (mInput.sqrMagnitude > 0.0001f)
                lastMoveDir = mInput.normalized;

            if (Input.GetButtonDown("Fire1"))
                Shoot();
        }

        // i-frames visual + timer
        if (invincible)
        {
            if (pSpriteRen)
            {
                float t = Mathf.PingPong(Time.time * flashSpeed, 1f); // 0..1..0..
                pSpriteRen.color = Color.Lerp(Color.white, flashColor, t);
            }

            if (Time.time >= invincibleUntil)
            {
                invincible = false;
                if (pSpriteRen)
                {
                    pSpriteRen.color = Color.white; // reset color at end
                    pSpriteRen.enabled = true;
                }
            }
        }

    }

    private void FixedUpdate()
    {
        // check timer for knockback for invinciblity
        if (isKnockBack && Time.time >= knockBackUntil)
        {
            isKnockBack = false;
            // optional: damp to zero instantly
            rigidBody.velocity = Vector2.zero;
        }

        if (isKnockBack)
        {
            rigidBody.velocity = knockBackVel;           // don't take input during knockback

        }
        else
        {
            rigidBody.velocity = mInput.normalized * mSpeed; // .normalize prevents diagonal from being at 1.4 speed of cardinal directions
        }
    }
            

    // If caller doesn't know the hit origin of object dealing damage, push opposite of lastMoveDir as a fallback
    public void TakeDamage(int damage)
    {
        Vector2 origin = (Vector2)transform.position - lastMoveDir;
        TakeDamage(damage, origin, true);
    }

    public void TakeDamage(int damage, Vector2 hitOrgin, bool doKnockback = true)
    {
        if (invincible) return; //can't take damage while invincible

        if ((curHealth - damage) > 0)
        {
            curHealth -= damage;
        }
        else
        {
            // character died
            Invoke(nameof(gameOver), 0.01f);
            return;
        }

        // Start i-frames
        invincible = true;
        invincibleUntil = Time.time + invincibleDuration;

        // after dealing damage, enter player into knockback
        if (doKnockback)
        {
            // get knockback direction vector
            Vector2 delta = (Vector2)transform.position-hitOrgin;
            Vector2 direction = delta.normalized;

            // implement knockback direction and set knockback time
            knockBackVel = direction * knockBackSpeed;
            isKnockBack = true;
            knockBackUntil = Time.time + knockBackDuration;
        }
    }

    public void GainHealth(int health)
    {
        // don't increase health if at max
        if (curHealth == maxHealth) return;
        // either increase by health, or increase to maxHealth
        else if ((curHealth + health) < maxHealth)
        {
            curHealth += health;
            return;
        }
        curHealth = maxHealth;
    }

    

    private void Shoot()
    {
        if (projectilePrefab == null) return; // should have a projectile prefab selected
        if (curMana == 0)  // only can fire if player has manna (1 proj = 1 mana)
        {
            // TODO: Add sound effect
            return;
        }
        curMana--;

        Vector3 spawnPos = firepoint ? firepoint.position : transform.position;
        GameObject go = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        var proj = go.GetComponent<Projectile>();
        proj.damage = projDamage; // matches player upgrades
        if (proj != null)
        {
            var myCol = GetComponent<Collider2D>();
            proj.Fire(lastMoveDir, true, myCol);    // calls fire
        }
    }

    // For when player interacts with chest key object
    public void GrabChestKey()
    {
        chestKeys ++;
        //TODO: Add pickup noise
    }

    public void UseChestKey()
    {
        chestKeys --;
    }

    // Return true if player has a chest key
    public bool CanOpenChest()
    {
        if (chestKeys == 0) return false;
        chestKeys--;
        return true;
    }

    // get level key
    public void GrabLevelKey()
    {
        if (levelKey)
        {
            // TODO: add debug message shouldn't be able to get 2 level keys
        }
        levelKey = true; 
        //TODO: add sound effect
    }

    // Uses level key on door
    public bool OpenDoor()
    {
        if (!levelKey) 
        {
            //TODO: play sound effect
            return false; 
        }
        else
        {
            //TODO: play sound effect
            return true;
        }
    }

    // increase mana
    public void IncreaseMana(int manaIncrease)
    {
        maxMana += manaIncrease;
        curMana += manaIncrease;
    }

    // increase fireball damage
    public void IncreaseDamage(int damageBoost)
    {
        projDamage += damageBoost;
    }

    public void IncreaseHealth(int healthBoost)
    {
        // increase a heart of health to both max and current
        maxHealth += healthBoost; 
        curHealth += healthBoost;
    }

    // called to regain mana periodically by a start function
    private void ManaRecovery()
    {
        if (curMana == maxMana) return;
        curMana ++;
    }

    // called for picking up a mana potion
    public void GrabManaPotion(int manaRegained)
    {
        //TODO: add sound affect
        if (curMana == maxMana) return;
        if ((curMana + manaRegained) >= maxMana)
        {
            curMana = maxMana;
            return;
        }
        curMana+= manaRegained;
    }

    /*-------------------- Getters --------------------*/
    public int GetCurHealth() {  return curHealth; }

    public int GetMaxHealth() { return maxHealth; }
    
    public int GetCurMana() { return curMana; }
    
    public int GetMaxMana() { return maxMana; }
    
    public int GetChestKeyCount() { return chestKeys; }
    
    public bool GetLevelKey() { return levelKey; }
    /*-------------------------------------------------*/

    private void gameOver()
    {
        Destroy(gameObject);
        // TODO: Add some function that triggers game over scene
    }
}
