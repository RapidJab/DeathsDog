using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Item
{
    None, 
    Plant,
    MusicSheet
}

public class InteractItem : Interactable
{
    [SerializeField] private Item item;
    private SpriteRenderer highlightObject;
    [SerializeField] Material normalMat; 
    [SerializeField] Material highlightMat;

    private void Awake()
    {
        highlightObject = GetComponentInChildren<SpriteRenderer>(); 
        highlightObject.material = normalMat;
    }
    public override void InteractWith(PlayerInteract player)
    {
        VariableManager.Instance.heldItems.Add(item);
        gameObject.SetActive(false);
    }
    public override void Nearby()
    {
        highlightObject.material = highlightMat;
    }
    public override void Away()
    {
        highlightObject.material = normalMat;
    }
}
