using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Rigidbody2D rb;
    public float attack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetSpeed(Vector2 speed)
    {
        rb.velocity = speed;
    }
}
