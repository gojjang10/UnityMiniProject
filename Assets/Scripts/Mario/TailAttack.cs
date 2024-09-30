using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailAttack : MonoBehaviour
{
    [SerializeField] ObjectPool tailPool;
    [SerializeField] Transform muzzlepoint;
    [SerializeField] AudioClip tailAttack;
    [SerializeField] Animator animator;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            animator.Play("RaccoonTailAttack");
            SoundManager.Instance.PlaySFX(tailAttack);
            Attack();
        }
        else if(Input.GetKeyUp(KeyCode.X))
        {
            animator.StopPlayback();
        }
    }

    private void Attack()
    {
        PooledObject instance = tailPool.GetPool(muzzlepoint.position, muzzlepoint.rotation);
        if(instance == null)
        {
            return;
        }
    }
}
