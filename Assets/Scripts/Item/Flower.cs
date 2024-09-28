using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : Item
{
    [SerializeField] Collider2D col;

    private void Start()
    {
        Vector3 beforePos = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        Vector3 curPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        //transform.position = Vector3.Lerp(beforePos, curPos, 1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

}
