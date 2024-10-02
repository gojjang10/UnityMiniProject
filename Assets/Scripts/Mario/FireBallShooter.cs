using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallShooter : MonoBehaviour
{
    [SerializeField] ObjectPool fireBallPool;
    [SerializeField] Transform muzzlePoint;
    [SerializeField] float fireBallSpeed;
    [SerializeField] AudioClip fire;
    private Vector2 left = new Vector2(-1, -0.5f).normalized;       // 각도 조절
    private Vector2 right = new Vector2(1, -0.5f).normalized;       // 각도 조절
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
        SoundManager.Instance.PlaySFX(fire);        // 생성되면 파이어볼 사운드

        FireBall fireBall = instance.gameObject.GetComponent<FireBall>();
        if(fireBall != null)
        {
            Vector2 dir = GetComponent<SpriteRenderer>().flipX ? left : right;
            fireBall.SetSpeed(fireBallSpeed * dir);
        }

    }
}
