using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 4;    //default enemy health is 4
    public GameObject projectilePrefab;
    public float fireFreq = 2.0f; 

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(Fire), fireFreq, fireFreq);
    }

    // Update is called once per frame
    private void Fire()
    {
        if (projectilePrefab == null) return; // must have a prefab to fire

        GameObject go = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile proj = go.GetComponent<Projectile>();

        if (proj != null)
        {
            Vector2 randDir = Random.insideUnitCircle.normalized;
            if (randDir == Vector2.zero)
            {
                randDir = Vector2.down; // in case direction is 0, shoot down
            }

            var myColl = GetComponent<Collider2D>();
            proj.Fire(randDir, false, myColl);
        }
    }
}
