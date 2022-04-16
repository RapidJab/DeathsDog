using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected Dialogue dialogue;
    private void Awake()
    { 
        dialogue = Camera.main.GetComponentInChildren<Dialogue>();
    }
    public abstract void InteractWith(PlayerInteract player);

    public abstract void Nearby();
    public abstract void Away();
}
