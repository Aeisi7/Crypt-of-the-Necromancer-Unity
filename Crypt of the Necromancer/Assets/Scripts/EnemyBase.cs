using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyBase : MonoBehaviour
{
    [Header("Common")]
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected int colDamage = 1;   // damage enemy deals when coliding with player
    [SerializeField] protected LayerMask wallLayers;    // for detecting walls
    [SerializeField] protected Transform player;        // for tracking player location


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
