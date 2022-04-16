using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
public class VariableManager : MonoBehaviour
{
    // Singleton
    public static VariableManager Instance { get; private set; }
    [ShowOnly] public GameObject player;
    [HideInInspector] public PlayerInput input;
    [HideInInspector] public Dialogue dialogue;
    public int memoriesSeen = 0;
    [HideInInspector] public SceneTransition transition;
    public PlayerMovement playerMovement { get; private set; }
    public string nextScene;

    [SerializeField] [ShowOnly] private bool _isBarked = false;
    public bool isBarked { get => _isBarked; set => _isBarked = value; }
    [SerializeField] private bool _inDialogue = false;
    public bool inDialogue { get => _inDialogue; set => _inDialogue = value; }
    [SerializeField] private bool _inCutscene = false;
    public bool inCutscene { get => _inCutscene; set => _inDialogue = value; }
    [SerializeField] private bool _inMemory = false;
    public bool inMemory { get => _inMemory; set => _inMemory = value; }

    public List<Item> heldItems = new List<Item>();
    public int memoryItems;
    [SerializeField] private GameObject checkpointPrefab; 
    [SerializeField][ShowOnly] private Checkpoint _curCheckpoint = null;
    public Checkpoint curCheckpoint { get => _curCheckpoint; set => _curCheckpoint = value; }
    public TextMeshProUGUI text; 
    public uint memoriesInScene;

    private void Awake()
    {
        if (Instance == null)
        { 
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        player = GameObject.Find("Player"); 
        input = player.GetComponent<PlayerInput>();
        playerMovement = player.GetComponent<PlayerMovement>();
        dialogue = Camera.main.GetComponentInChildren<Dialogue>();
        transition = Camera.main.GetComponentInChildren<SceneTransition>();
    }

    private void Update()
    {
        text.text = memoryItems.ToString();
        transform.position = player.transform.position;
    }
}
