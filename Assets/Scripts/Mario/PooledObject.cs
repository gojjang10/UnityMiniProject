using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public ObjectPool returnPool;
    private float returnTime = 10;

    private float curTime;

    private void OnEnable()
    {
        curTime = returnTime;
    }

    private void Update()
    {
        curTime -= Time.deltaTime;
        if (curTime < 0)
        {
            returnPool.ReturnPool(this);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            returnPool.ReturnPool(this);
        }
       
    }
}
