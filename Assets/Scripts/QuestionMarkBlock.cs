using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionMarkBlock : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] AudioClip hitSound;
    [SerializeField] float height;

    private int count = 1;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && 
            count == 1 &&
            transform.position.y > collision.transform.position.y + height)
        {
            Debug.Log("[?]박스 충돌");
            animator.Play("QuestionHit");
            SoundManager.Instance.PlaySFX(hitSound);
            count--;
        }
        else if(transform.position.y > collision.transform.position.y && count == 0)
        {
            SoundManager.Instance.PlaySFX(hitSound);
        }
    }
}
