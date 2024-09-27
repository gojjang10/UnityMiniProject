using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionMarkBlock : MonoBehaviour
{

    [System.Serializable]
    public struct itemSlot
    {
        public MarioType mariostate;
        public Item Item;
    }

    [SerializeField] Animator animator;
    [SerializeField] AudioClip hitSound;
    [SerializeField] float height;
    
    public List<itemSlot> slots = new List<itemSlot>();

    private int count = 1;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && 
            count == 1 &&
            transform.position.y > collision.transform.position.y + height)
        {
            MarioController mario = collision.gameObject.GetComponent<MarioController>();
            if(mario != null)
            {
                    SpawnItem(mario);
            }
            BoxOnHit();
        }
        else if(transform.position.y > collision.transform.position.y && count == 0)
        {
            SoundManager.Instance.PlaySFX(hitSound);
        }
    }

    private void BoxOnHit()
    {
        Debug.Log("[?]박스 충돌");
        animator.Play("QuestionHit");
        SoundManager.Instance.PlaySFX(hitSound);
        count--;
    }

    private void SpawnItem(MarioController mario)
    {
        foreach(itemSlot slot in slots)
        {
            if(slot.mariostate == mario.curMarioType)
            {
                Instantiate(slot.Item, transform.position + Vector3.up, Quaternion.identity);
                Debug.Log("아이템 생성");
                break;
            }
        }
    }

    

    
}
