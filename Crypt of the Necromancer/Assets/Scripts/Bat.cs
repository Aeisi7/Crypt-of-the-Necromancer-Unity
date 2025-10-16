using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : EnemyBase
{
    [Header("Movement")]
    [SerializeField] private float changeDirChance = 0.1f;
    [SerializeField] private float reverseJitterDeg = 30f;

    private Vector2 dir = Vector2.zero;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        dir = Random.insideUnitCircle.normalized;
        if (dir == Vector2.zero) dir = Vector2.right;
    }

    protected override Vector2 GetDirection()
    {
        if (Random.value < changeDirChance || dir == Vector2.zero)
        {
            dir = Random.insideUnitCircle.normalized;
            if (dir == Vector2.zero) dir = Vector2.right;
        }

        // handle small numbers to avoid returning nan
        if (dir.sqrMagnitude < 1e-6f) dir = Vector2.right;

        return dir;
    }

    protected override void OnBlocked()
    {
        // Cast to detect wall and get its normal (will allow reflection 20 degrees in each direction from normal)
        Vector2 origin = col.bounds.center; // get origin of bat
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, wallCheckDistance, wallLayers);
        if (!hit.collider) // in case of weird physics overlap
        {
            dir = -dir; // fallback top totally reversing to be safe

            // move the bat slightly to not be stuck on the wall the next frame
            transform.position += (Vector3)(-dir * 0.03f); // move 3 frames (100ppu)
            return;
        }

        // Perfect mirror reflection relative to wall normal
        Vector2 reflected = Vector2.Reflect(dir, hit.normal).normalized;

        // Apply jitter *around the reflection angle*
        float jitter = Random.Range(-reverseJitterDeg, reverseJitterDeg);
        dir = Rotate(reflected, jitter).normalized;

        // handle small numbers to avoid returning nan
        if (dir.sqrMagnitude < 1e-6f) dir = reflected;

        transform.position += (Vector3)(hit.normal * 0.03f); // move 3 frames (100ppu)
    }

    // Used to rotate a 2D vector with degrees given (used for wall collision)
    private static Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad; // convert to radians for use in math/vector functions
        float cs = Mathf.Cos(rad);
        float sn = Mathf.Sin(rad);
        return new Vector2(v.x * cs - v.y * sn, v.x * sn + v.y * cs);
    }
}

