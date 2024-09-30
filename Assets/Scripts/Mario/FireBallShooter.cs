using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallShooter : MonoBehaviour
{
    [SerializeField] ObjectPool fireBallPool;
    [SerializeField] Transform muzzlePoint;
    [SerializeField] float fireBallSpeed;
    [SerializeField] AudioClip fire;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {

            SoundManager.Instance.PlaySFX(fire);
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
            Vector2 dir = GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right;
            fireBall.SetSpeed(fireBallSpeed * dir);
        }

    }
}
