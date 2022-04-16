using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingTrap : MonoBehaviour
{
    [SerializeField] private float fallDelay;
    [SerializeField] private bool respawn;
    [SerializeField] private float respawnDelay;
    [SerializeField] [ShowOnly] private bool oneWay;
    private Collider2D col;

    private Vector2 initialPos; 
    private Rigidbody2D rb2d;
    [SerializeField] private Transform spriteObject;
    private Vector2 initialSpritePos;
    private Vector2 randomPos;

    [Header("Settings")] 
    [Range(0f, 2f)]
    [SerializeField] private float _distance = 0.1f;
    [Range(0f, 0.1f)]
    [SerializeField] private float _delayBetweenShakes = 0f;
    [SerializeField] private bool stationary = true;
    [SerializeField] float gravityScale;

    private AudioSource shakeAudio;
     
    private void Start()
    {
        oneWay = GetComponent<PlatformEffector2D>().enabled;
        col = GetComponent<Collider2D>();
        initialPos = transform.position;
        initialSpritePos = spriteObject.position;
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 0;
        rb2d.isKinematic = true;
        shakeAudio = gameObject.GetComponent<AudioSource>();
    }  

    private IEnumerator Shake()
    {
        float timer = 0f;
        shakeAudio.Play();
        while (timer < fallDelay)
        {
            timer += Time.deltaTime;

            //MusicManager.Instance.PlaySound(MusicManager.Instance.SetRandomFallingPlatformSound(), .2f);

            randomPos = initialSpritePos + (Random.insideUnitCircle * _distance);

            spriteObject.position = randomPos;

            if (_delayBetweenShakes > 0f)
            {
                yield return new WaitForSeconds(_delayBetweenShakes);
            }
            else
            {
                yield return null;
            }
        }
        shakeAudio.Stop();

        spriteObject.position = initialSpritePos;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!stationary)
            return;
        if (VariableManager.Instance.playerMovement.grounded == gameObject)
        {
            StartCoroutine(waitForFall());
        }
    } 
    public void fall()
    {
        if (!oneWay)
            col.isTrigger = true;
        rb2d.isKinematic = false;
        rb2d.gravityScale = 1.2f; 
        if (respawn)
        {
            StartCoroutine(waitForRespawn());
        }
    } 
    private IEnumerator waitForFall()
    {
        stationary = false;
        StartCoroutine(Shake());
        yield return new WaitForSeconds(fallDelay);
        fall();  
    } 

    private IEnumerator waitForRespawn()
    {
        yield return new WaitForSeconds(respawnDelay); 
        rb2d.velocity = Vector2.zero;
        rb2d.gravityScale = 0;
        rb2d.isKinematic = true;
        transform.position = initialPos;
        stationary = true;
        if (!oneWay)
            col.isTrigger = false;
    }
}
