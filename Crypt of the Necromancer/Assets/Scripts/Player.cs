using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{

    // used by gamemanager
    [System.Serializable]
    public class PlayerData
    {
        public float mSpeed;
        public int maxHealth, curHealth;
        public int chestKeys;
        public int projDamage;
        public int maxMana, curMana;
        public bool levelKey;
    }

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


    [Header("SoundFX")]
    [SerializeField] private AudioClip keyPickupSound;
    [SerializeField] private AudioClip drinkPotionSound;
    [SerializeField] private AudioClip projSound;
    [SerializeField] private AudioClip doorLockedSound;
    [SerializeField] private AudioClip hitSound;

    bool isDead = false;

    // knockback vars
    private bool isKnockBack = false;
    private Vector2 knockBackVel;
    private float knockBackUntil = 0f; // will point of time that knockback last until (will use Time.time)

    private bool invincible = false;
    private float invincibleUntil = 0f;
    private bool readingSign = false;

    [Header("Damage Effects")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashSpeed = 14f; 

    private SpriteRenderer pSpriteRen;
    private Rigidbody2D rigidBody;
    private Vector2 mInput;     // movement input
    private Vector2 lastMoveDir = Vector2.up; // default shoot direction is up


    // ---------- use gamemaker to handel scene transitions -----------------
    public PlayerData ToData()
    {
        return new PlayerData
        {
            mSpeed = mSpeed,
            maxHealth = maxHealth,
            curHealth = curHealth,
            chestKeys = chestKeys,
            projDamage = projDamage,
            maxMana = maxMana,
            curMana = curMana,
            levelKey = false // on new scene, always lose levelkey
        };
    }

    public void FromData(PlayerData d)
    {
        mSpeed = d.mSpeed;
        maxHealth = d.maxHealth;
        curHealth = Mathf.Clamp(d.curHealth, 0, maxHealth);
        chestKeys = Mathf.Max(0, d.chestKeys);
        projDamage = d.projDamage;
        maxMana = d.maxMana;
        curMana = Mathf.Clamp(d.curMana, 0, maxMana);
        levelKey = d.levelKey;
    }

    // ----------------------------------------------------------------------

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
            {
                lastMoveDir = mInput.normalized;
                // Add walking loop function here, may need to check if currently playing a walking noise (boolean)
            }

            if (readingSign) return; // don't allow player to shoot while reading sign
            
            if (Input.GetButtonDown("Fire1"))
                Shoot();
        }

        // i-frames visual + timer
        if (invincible)
        {
            if (readingSign)
            {
                pSpriteRen.color = Color.white;
                return; // avoid turning off invinciblity while reading sign
            }
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
        if (isDead)
        {
            rigidBody.velocity = Vector2.zero;
            return;
        }

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
            // prevents weird UI/camera stuff on death
            curHealth = 0;
            rigidBody.velocity = Vector2.zero;
            Collider2D col = GetComponent<Collider2D>();
            col.enabled = false;
            doKnockback = false;

            // character died
            Invoke(nameof(gameOver), 0.01f);
            return;
        }

        // SoundFX
        SoundFXManager.Instance.PlaySoundFXClip(hitSound, transform, 1f);

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

            // playes sound effect
            SoundFXManager.Instance.PlaySoundFXClip(projSound, transform, 0.6f);
        }
    }

    // for reading signs (if more time pause game and allow exit some how instead)
    public void SetInvincible(bool value)
    {
        invincible = value;
        // set reading sign so that enemies can't hit player (stops update from overwriting)
        readingSign = value;
    }

    // For when player interacts with chest key object
    public void GrabChestKey()
    {
        chestKeys ++;
        // pickup noise
        SoundFXManager.Instance.PlaySoundFXClip(keyPickupSound, transform, 1f);
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
            Debug.Log("Somehow got 2 level keys"); return;

        }
        levelKey = true; 
        
    }

    // Uses level key on door
    public void OpenDoor()
    {
        //if (!levelKey) 
        //{
            SoundFXManager.Instance.PlaySoundFXClip(doorLockedSound, transform, 1f);
            //return false; 
        //}
        //else
        //{
            
        //    return true;
        //}
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

    // used when opening a health bonus chest
    public void IncreaseHealth(int healthBoost)
    {

        // increase a max health and current health 
        maxHealth += healthBoost;
        curHealth += healthBoost;
    }

    public void GrabHealthPotion(int healthRecover)
    {
        // pickup noise
        SoundFXManager.Instance.PlaySoundFXClip(drinkPotionSound, transform, 1f);
        GainHealth(healthRecover);
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
        // pickup noise
        SoundFXManager.Instance.PlaySoundFXClip(drinkPotionSound, transform, 1f);
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

    public int GetProjDamage() { return projDamage; }

    public bool GetLevelKey() { return levelKey; }

    public float GetSpeed() { return mSpeed; }
    /*-------------------------------------------------*/

    private void gameOver()
    {
        isDead = true;
        Destroy(gameObject);
        // TODO: Add some function that triggers game over scene
        SceneManager.LoadScene("GameOverMenu");
    }
}
