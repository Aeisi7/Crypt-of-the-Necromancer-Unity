using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float mSpeed = 0.5f;   // player movement speed
    public GameObject projectilePrefab;
    public Transform firepoint;
    public int health = 12;

    private Rigidbody2D rigidBody;
    private Vector2 mInput;     // movement input
    private Vector2 lastMoveDir = Vector2.up; // default shoot direction is up



    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>(); // initializes rigidBody on start
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

    private void FixedUpdate()
    {
        rigidBody.velocity = mInput.normalized * mSpeed; // .normalize prevents diagonal from being at 1.4 speed of cardinal directions
    }

    private void Shoot()
    {
        if (projectilePrefab == null) return; // should have a projectile prefab selected

        Vector3 spawnPos = firepoint ? firepoint.position : transform.position;
        GameObject go = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        var proj = go.GetComponent<Projectile>();
        if (proj != null)
        {
            var myCol = GetComponent<Collider2D>();
            proj.Fire(lastMoveDir, true, myCol);    // calls fire
        }
    }
}
