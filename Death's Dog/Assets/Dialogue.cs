using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using TMPro;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{ 
    [SerializeField] private AudioClip textScrollAudio;
    [SerializeField] private AudioClip textboxAudio;
    private TextMeshProUGUI ui;
    private Image[] textbox;
    public TextLine[] lines;
    public TextLine[] yesLines;
    public InteractMemory curMemory;
    public InteractCharacter curCharacter;
    private int index;
    [SerializeField] private Animator[] actors; 
    private bool waiting;
    private float requiredTime;
    public GameObject yes;
    public GameObject no;
    CameraFollow camFollow; 
    public Coroutine textEnumerator;
    public bool beginCredits;
    public float creditLength;
    public bool creditsGoing;

    private void Awake()
    {
        camFollow = Camera.main.gameObject.GetComponent<CameraFollow>();
        textbox = GetComponentsInChildren<Image>();
        ui = GetComponentInChildren<TextMeshProUGUI>();
        ui.text = string.Empty;
        foreach(Image box in textbox)
        {
            box.enabled = false; 
        } 
        yes.gameObject.SetActive(false);
        no.gameObject.SetActive(false); 
    } 

    private IEnumerator BeginCredits()
    {
        creditsGoing = true;
        ui.text = string.Empty;
        foreach (Image box in textbox)
        {
            box.enabled = false;
        }
        yield return new WaitForSeconds(creditLength);
        StartCoroutine(VariableManager.Instance.transition.ExitScene()); 
    }

    void Update()
    {
        if (Time.timeScale == 0) 
            return; 
        if (VariableManager.Instance.playerMovement.grounded && lines != null && lines.Length > 0 && !creditsGoing) //Player is grounded
        {  
            if (ui.text == lines[index].line && lines[index].yesNo)
            {
                yes.gameObject.SetActive(true);
                no.gameObject.SetActive(true);
            } 
            if(yes.activeInHierarchy && VariableManager.Instance.input.currentActionMap.FindAction("Yes").GetButtonDown())
            {
                YesDialogue();
            } else if (yes.activeInHierarchy && VariableManager.Instance.input.currentActionMap.FindAction("No").GetButtonDown())
            { 
                if (camFollow.curEnumerator != null)
                {
                    camFollow.StopCoroutine(camFollow.curEnumerator);
                }
                camFollow.curEnumerator = camFollow.StartCoroutine(camFollow.ToReality(VariableManager.Instance.player.transform));
                VariableManager.Instance.inDialogue = false; //Stop input from working 
                foreach (Image box in textbox)
                {
                    box.enabled = false;
                }
                ui.text = string.Empty;
                lines = null;
                yes.gameObject.SetActive(false);
                no.gameObject.SetActive(false);
                return;
            }
            if (VariableManager.Instance.input.currentActionMap.FindAction("NextText").GetButtonDown()) //
            { 
                if (ui.text == lines[index].line && lines[index].yesNo)
                { 
                }
                else if (ui.text == lines[index].line && (requiredTime <= 0 || beginCredits))
                {
                    NextLine();
                }  
                else if(!waiting)
                { 
                    if (textEnumerator != null)
                        StopCoroutine(textEnumerator); 
                    ui.text = lines[index].line;
                }
            }
        }
        if(requiredTime > 0)
        {
            requiredTime -= Time.deltaTime;
        }
    }

    public void StartDialogue()
    {
        foreach (Image box in textbox)
        {
            box.enabled = true;
        }
        yes.gameObject.SetActive(false);
        no.gameObject.SetActive(false);
        ui.text = string.Empty;
        gameObject.SetActive(true); 
        index = 0;  
        MusicManager.Instance.PlaySound(textboxAudio);
        if (textEnumerator != null)
            StopCoroutine(textEnumerator);
        textEnumerator = StartCoroutine(PrintLine());
    }

    private Animator FindActor(string actorName)
    {
        foreach(Animator actor in actors)
        {
            if(actor.gameObject.name == actorName)
            {
                return actor;
            }
        }
        return null;
    }

    private IEnumerator PrintLine()
    {   
        if(lines[index].moveSpeed != 0)
        {
            StartCoroutine(MoveActor(FindActor(lines[index].actorName).transform, lines[index].moveSpeed, lines[index].requiredTime));
        }
        if (lines.Length > index)
        {
            foreach (Image box in textbox)
            {
                box.color = lines[index].boxColor; //Set the texbox color
            } 
            ui.color = lines[index].textColor; //Set the text color
        }
        if (lines[index].animName != "") //Play an animation
        {
            FindActor(lines[index].actorName).SetBool(lines[index].animName, true);
        }
        requiredTime = lines[index].requiredTime;
        foreach (char c in lines[index].line.ToCharArray())
        {
            MusicManager.Instance.PlaySound(textScrollAudio, .4f, Random.Range(.9f, 1.1f));
            ui.text += c;
            yield return new WaitForSeconds(lines[index].scrollSpeed);
        }
        if (ui.text == lines[index].line && lines[index].yesNo)
        {
            yes.gameObject.SetActive(true);
            no.gameObject.SetActive(true);
        }
    }

    public void NextLine()
    { 
        if (index >= lines.Length - 1 && beginCredits)
        {
            Debug.Log("Yeah");
            StartCoroutine(BeginCredits());
            return;
        } 
        yes.gameObject.SetActive(false);
        no.gameObject.SetActive(false);
        if (lines[index].animName != "") //Play an animation
        {
            FindActor(lines[index].actorName).SetBool(lines[index].animName, false);
        }
        MusicManager.Instance.PlaySound(textboxAudio, .4f); 
        ui.text = string.Empty;
        if (index < lines.Length - 1)
        { 
            index++;
            if (textEnumerator != null)
                StopCoroutine(textEnumerator);
            textEnumerator = StartCoroutine(PrintLine());
        } else
        {
            if(VariableManager.Instance.memoriesSeen == VariableManager.Instance.memoriesInScene && VariableManager.Instance.memoriesInScene != 0)
            {
                index = 0;
                VariableManager.Instance.memoriesSeen++;
                lines = new TextLine[] { (TextLine)Resources.Load("AllMemoriesSeen") }; 
                if (textEnumerator != null)
                    StopCoroutine(textEnumerator);
                textEnumerator = StartCoroutine(PrintLine());
                return;
            }
            if (camFollow.curEnumerator != null)
            {
                camFollow.StopCoroutine(camFollow.curEnumerator);
            }
            camFollow.curEnumerator = camFollow.StartCoroutine(camFollow.ToReality(VariableManager.Instance.player.transform)); 
            VariableManager.Instance.inDialogue = false; //Stop input from working 
            foreach (Image box in textbox)
            {
                box.enabled = false;
            }
            lines = null; 
        }
    } 
    public void YesDialogue()
    {
        if (curMemory)
        {
            curMemory.StartMemory();
            StartDialogue();
        }
        else
        {
            NextLine();
        }
    }

    public IEnumerator MoveActor(Transform actor, float speed, float time)
    {
        while(time > 0)
        {
            Vector2 newPos = actor.position;
            newPos.x += speed * Time.deltaTime;
            actor.position = newPos;
            time -= Time.deltaTime;
            yield return null;
        }
    }
}