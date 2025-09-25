using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float mSpeed = 10f;   // player movement speed
    private Rigidbody2D rigidBody;
    private Vector2 mInput;     // movement input

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
    }

    private void FixedUpdate()
    {
        rigidBody.velocity = mInput.normalized * mSpeed; // .normalize prevents diagonal from being at 1.4 speed of cardinal directions
    }
}
