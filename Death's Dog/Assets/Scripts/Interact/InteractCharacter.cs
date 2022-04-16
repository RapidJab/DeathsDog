using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractCharacter : Interactable
{ 
    private SpriteRenderer highlightObject;
    [SerializeField] Material normalMat;
    [SerializeField] Material highlightMat;
    public TextLine[] lines;
    [SerializeField] private int camZoom;
    public Transform camPos;
    [SerializeField] private float talkCooldown = .3f;
    private CameraFollow camFollow;
    public List<Coroutine> startCoroutines;
    public List<Coroutine> endCoroutines;
    [SerializeField] private float alphaChange = 0.03f;
    SpriteRenderer rend;
    [SerializeField] bool destroyAfterLast;
    [SerializeField] bool transitionToNextSceen;
    [SerializeField] bool beginCredits;

    [SerializeField] private bool facingRight;
    SpriteRenderer ownRend;
    [SerializeField] GameObject tutObj; 

    private void Awake()
    {
        ownRend = GetComponentInChildren<SpriteRenderer>();
        startCoroutines = new List<Coroutine>();
        endCoroutines = new List<Coroutine>();
        highlightObject = GetComponentInChildren<SpriteRenderer>();
        highlightObject.material = normalMat;
        camFollow = Camera.main.gameObject.GetComponent<CameraFollow>();
        rend = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (!VariableManager.Instance.playerMovement.sitting)
        {
            if (VariableManager.Instance.player.transform.position.x > transform.position.x && !facingRight)
            {
                rend.flipX = true;
                facingRight = true;
            }
            else if (VariableManager.Instance.player.transform.position.x <= transform.position.x && facingRight)
            {
                rend.flipX = false;
                facingRight = false;
            }
        }
        else
        {
            // rend.flipX = false;
        }
    }
    public override void InteractWith(PlayerInteract player)
    {
        if (tutObj)
            tutObj.SetActive(false);
        VariableManager.Instance.dialogue.curMemory = null;
        gameObject.layer = LayerMask.NameToLayer("Default"); 
        if (camFollow.curEnumerator != null)
            camFollow.StopCoroutine(camFollow.curEnumerator);
        VariableManager.Instance.dialogue.curCharacter = this;
        VariableManager.Instance.dialogue.lines = lines;
        VariableManager.Instance.inDialogue = true; //Begin dialogue 
        camFollow.curEnumerator = camFollow.StartCoroutine(camFollow.ToMemory(camPos, camZoom));
        VariableManager.Instance.dialogue.beginCredits = beginCredits;
    }
    public override void Nearby()
    {
        highlightObject.material = highlightMat;
    }
    public override void Away()
    {
        highlightObject.material = normalMat;
    }
    public IEnumerator EnterScene()
    {
        foreach (Coroutine routine in endCoroutines)
        {
            StopCoroutine(routine);
        }
        yield return null;
    }

    public IEnumerator ExitScene()
    { 
        foreach (Coroutine routine in startCoroutines)
        {
            StopCoroutine(routine);
        }
        yield return new WaitForSeconds(talkCooldown);

        // if only able to talk once
        if (destroyAfterLast)
        {
            while (rend.color.a > 0)
            {
                Debug.Log("Reducing alfa");
                Color tempColor = rend.color;
                tempColor.a -= alphaChange;
                rend.color = tempColor;
                yield return null;
            }
            gameObject.SetActive(false);
        }

        if (transitionToNextSceen)
        {
            Debug.Log("To next scene");
            StartCoroutine(VariableManager.Instance.transition.ExitScene());
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Interact");
        } 
    }

    public void DeactivateObject()
    {
        gameObject.SetActive(false);
    }
}
