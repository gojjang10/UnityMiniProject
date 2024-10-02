using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public ObjectPool returnPool;
    [SerializeField] private float returnTime = 2f;

    [SerializeField] private float curTime;

    private void OnEnable()
    {
        curTime = returnTime;
    }

    private void Update()
    {
        curTime -= Time.deltaTime;
        Debug.Log($"현재 시간{curTime}");
        if (curTime <= 0)
        {
            returnPool.ReturnPool(this);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Goomba"))
        {
            returnPool.ReturnPool(this);
        }
       
    }
}
