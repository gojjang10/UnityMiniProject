using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

public class CoinQuestionMarkBlock : MonoBehaviour
{
    [SerializeField] GameObject coin;
    [SerializeField] Animator coinAnimator;
    [SerializeField] Animator animator;
    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip coinSound;
    [SerializeField] float height;


    private int count = 1;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") &&
            count == 1 &&
            transform.position.y > collision.transform.position.y + height)
        {
            BoxOnHit();
            GameObject instance = Instantiate(coin, transform.position + Vector3.up * 1f, Quaternion.identity);     // 코인 위로 생성
            SoundManager.Instance.PlaySFX(coinSound);                                                               // 코인 효과음 발생
            instance.GetComponent<Animator>().Play("HitCoin");                                                      // 코인 애니메이션 실행
            Debug.Log("코인획득");
            GameManager.Instance.coin++;
            Destroy(instance, 0.5f);                                                                                // 코인 삭제
        }
  
    }

    private void BoxOnHit()
    {
        Debug.Log("[?]박스 충돌");
        animator.Play("QuestionHit");
        SoundManager.Instance.PlaySFX(hitSound);
        count--;
    }

}
