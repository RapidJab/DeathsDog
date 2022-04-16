using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private SpriteRenderer rend;
    [SerializeField] private AudioClip sound;
    [SerializeField] private float disappearSpeed;
    private bool taken;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.root.gameObject == VariableManager.Instance.player && !taken)
        {
            taken = true;
            float randPitch = Random.Range(.85f, 1.15f);
            MusicManager.Instance.PlaySound(sound, .3f, randPitch);
            VariableManager.Instance.memoryItems++;
            StartCoroutine(Disappear());
        }
    }

    IEnumerator Disappear()
    { 
        while(rend.color.a > 0)
        {
            Color temp = rend.color;
            temp.a -= Time.deltaTime * disappearSpeed;
            rend.color = temp;
            yield return null;
        }
        Destroy(gameObject);
    }
}
