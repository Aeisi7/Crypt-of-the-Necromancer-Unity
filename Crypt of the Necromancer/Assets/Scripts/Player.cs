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


    private Rigidbody2D rigidBody;
    private Vector2 mInput;     // movement input
    private Vector2 lastMoveDir = Vector2.up; // default shoot direction is up



    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>(); // initializes rigidBody on start
        InvokeRepeating(nameof(ManaRecovery), manaRechargeSpeed, manaRechargeSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        mInput.x = Input.GetAxisRaw("Horizontal");
        mInput.y = Input.GetAxisRaw("Vertical");

        if (mInput.sqrMagnitude > 0.0001f)
        {
            lastMoveDir = mInput.normalized;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }


    public void TakeDamage(int damage)
    {
        if ((curHealth - damage) > 0)
        {
            curHealth -= damage;
            return;
        }

        // character died
        Invoke(nameof(gameOver), 0.01f);
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

    private void FixedUpdate()
    {
        rigidBody.velocity = mInput.normalized * mSpeed; // .normalize prevents diagonal from being at 1.4 speed of cardinal directions
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

    // increase movement speed
    public void IncreaseMoveSpeed(float speedBoost)
    {
        mSpeed += speedBoost;
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

    private void gameOver()
    {
        Destroy(gameObject);
        // TODO: Add some function that triggers game over scene
    }
}
