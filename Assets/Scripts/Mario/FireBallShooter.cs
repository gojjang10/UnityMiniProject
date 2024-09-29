using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallShooter : MonoBehaviour
{
    [SerializeField] ObjectPool fireBallPool;
    [SerializeField] Transform muzzlePoint;
    [SerializeField] float fireBallSpeed;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            Fire();
        }
    }

    private void Fire()
    {
        PooledObject instance = fireBallPool.GetPool(muzzlePoint.position, muzzlePoint.rotation);
        if(instance == null)
        {
            return;
        }
        FireBall fireBall = instance.gameObject.AddComponent<FireBall>();
        if(fireBall != null)
        {
            fireBall.SetSpeed(fireBallSpeed * muzzlePoint.forward);
        }

    }
}
