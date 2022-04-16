using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersOnEnd : MonoBehaviour
{
    [ShowOnly] bool setCharactersActive = false; 
    [SerializeField] List<GameObject> charactersOn;
    [SerializeField] GameObject[] charactersOff;
    // Start is called before the first frame update
    private void Awake()
    {
        charactersOn = new List<GameObject> { };
        foreach(Transform child in transform)
        {
            charactersOn.Add(child.gameObject);
        }
        
    }

    void Start()
    {

        foreach (GameObject character in charactersOn)
        {
            character.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!setCharactersActive && VariableManager.Instance.memoriesSeen >= VariableManager.Instance.memoriesInScene)
        {
            foreach (GameObject character in charactersOff)
            {
                character.SetActive(false);
            }
            foreach (GameObject character in charactersOn)
            {
                character.SetActive(true);
            }

        }
    }
}
