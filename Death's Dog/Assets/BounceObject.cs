using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceObject : MonoBehaviour
{
    [SerializeField] private float bouncePower;
    [SerializeField] private AudioClip sound;
    private Animator anim;
    private float timer;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.enabled && timer <= 0)
        {
            float randPitch = Random.Range(.97f, 1.03f);
            MusicManager.Instance.PlaySound(sound, .3f, randPitch);
            VariableManager.Instance.playerMovement.ReplenishJumps(); 
            Rigidbody2D rb2d = collision.gameObject.GetComponent<Rigidbody2D>();
            rb2d.AddForce(collision.gameObject.transform.up * bouncePower);
            anim.SetTrigger("IsBounce");
            timer = .1f;
        }
    }
}
