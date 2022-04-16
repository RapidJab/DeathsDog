using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractMemory : Interactable
{
    private SpriteRenderer highlightObject;
    [SerializeField] Material normalMat;
    [SerializeField] Material highlightMat;
    [SerializeField] private int requiredItems;
    public TextLine[] failLines;
    public TextLine[] promptLines;
    public SpriteRenderer[] memoryObjects;
    [HideInInspector] public Vector3[] objectsPos;
    [HideInInspector] public int[] xScale;
    public TextLine[] memoryLines;
    [SerializeField] private float alphaChange; 
    [SerializeField] private int camZoom;
    public Transform camPos;
    private CameraFollow camFollow; 
    public List<Coroutine> startCoroutines;
    public List<Coroutine> endCoroutines;
    [ShowOnly] private bool memorySeen = false;
    [SerializeField] private bool facingRight;
    SpriteRenderer rend; 

    private void Awake()
    {
        rend = GetComponentInChildren<SpriteRenderer>();
        startCoroutines = new List<Coroutine>();
        endCoroutines = new List<Coroutine>();
        highlightObject = GetComponentInChildren<SpriteRenderer>(); 
        highlightObject.material = normalMat;
        camFollow = Camera.main.gameObject.GetComponent<CameraFollow>();
        foreach(SpriteRenderer rend in memoryObjects)
        {
            Color invis = rend.color;
            invis.a = 0;
            rend.color = invis;
        }
        objectsPos = new Vector3[memoryObjects.Length];
        for (int i = 0; i < memoryObjects.Length; i++)
        {
            objectsPos[i] = memoryObjects[i].transform.localPosition;
        }
        xScale = new int[memoryObjects.Length];
        for (int i = 0; i < memoryObjects.Length; i++)
        {
            xScale[i] = (int)memoryObjects[i].transform.localScale.x;
        }
    }
    public override void InteractWith(PlayerInteract player)
    {
        VariableManager.Instance.dialogue.curMemory = null;
        if (VariableManager.Instance.memoryItems >= requiredItems)
        { 
            gameObject.layer = LayerMask.NameToLayer("Default");
            if(camFollow.curEnumerator != null)
                camFollow.StopCoroutine(camFollow.curEnumerator);
            if (promptLines != null)
            {
                VariableManager.Instance.dialogue.lines = promptLines;
            } 
            VariableManager.Instance.dialogue.curMemory = this;
            VariableManager.Instance.inDialogue = true; //Begin dialogue 
            camFollow.curEnumerator = camFollow.StartCoroutine(camFollow.ToMemory(camPos, camZoom));
        }
        else
        {
            VariableManager.Instance.dialogue.lines = failLines;
            VariableManager.Instance.inDialogue = true; //Begin dialogue 
        }
    }

    private void Update()
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
    public override void Nearby()
    {
        highlightObject.material = highlightMat;
    }
    public override void Away()
    {
        highlightObject.material = normalMat;
    }
    public void StartMemory()
    {
        if (!memorySeen)
        {
            promptLines = new TextLine[] { (TextLine)Resources.Load("finalPromptLine") };
            VariableManager.Instance.memoriesSeen++;
            memorySeen = true;
        }
        VariableManager.Instance.memoryItems -= requiredItems;
        requiredItems = 0; 
        VariableManager.Instance.dialogue.lines = memoryLines; 
        VariableManager.Instance.inDialogue = true; //Begin dialogue 
    } 
    public IEnumerator EnterScene(SpriteRenderer rend)
    { 
        foreach (Coroutine routine in endCoroutines)
        {
            StopCoroutine(routine); 
        }
        while (rend.color.a < 1)
        { 
            Color tempColor = rend.color;
            tempColor.a += alphaChange;
            rend.color = tempColor;
            yield return null;
        }
    }
    public IEnumerator ExitScene(SpriteRenderer rend, Vector3 pos, int x)
    {
        foreach(Coroutine routine in startCoroutines)
        {
            StopCoroutine(routine); 
        }
        //VariableManager.Instance.dialogue.curMemory = null;
        while (rend.color.a > 0)
        { 
            Color tempColor = rend.color;
            tempColor.a -= alphaChange; 
            rend.color = tempColor; 
            yield return null;
        }
        rend.transform.localPosition = pos;
        Vector3 scaleX = rend.transform.localScale;
        scaleX.x = x;
        rend.transform.localScale = scaleX;

         

        gameObject.layer = LayerMask.NameToLayer("Interact");  
    }
} 
