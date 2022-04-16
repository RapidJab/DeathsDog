using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barkObject : MonoBehaviour
{
    SpriteRenderer[] spriteRenderers;
    [SerializeField] Color transparentColor;
    [SerializeField] Color solidColor;
    [SerializeField] bool enabledOnBark;
    public Collider2D barkCollider;
    [SerializeField][ShowOnly] private int collisionCount = 0;
    [ShowOnly] public bool inBarkRange = false; 

    void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        Collider2D[] tempColliders = GetComponents<Collider2D>();
        foreach(Collider2D col in tempColliders)
        {
            if(!col.isTrigger)
            { 
                barkCollider = GetComponent<Collider2D>();
                break;
            }
        } 
        barkCollider.enabled = !enabledOnBark; 

        foreach (SpriteRenderer childRender in spriteRenderers)
        { 
            childRender.color = (enabledOnBark ? transparentColor : solidColor);
        }
    }
     
    void Update()
    {
        /*
         * If something is in the object, do nothing 
         * If this object is in range and isBarked, turn it to the opposite of what it's supposed to be
         * Variables:
         * isBarked - to know whether bark is going
         * enabledOnBark - If false, object exists normally and disappears when barked
         * currentState - If true, object exists, if false, object is disappeared
         * inBarkRange - To know if the object should be changed from isBarked
         * Check if isBarked and inBarkRange
         *    You are, so check if your current state equals enabledOnBark
         *       It doesn't, so change to be enabledOnBark
         *    You aren't, so check if your current state equals your enabledOnBark
         *       It does, so change to be not enabledOnBark
        */
        if (collisionCount == 0)
        {
            if (VariableManager.Instance.isBarked)
            {
                if(barkCollider.enabled != enabledOnBark && inBarkRange)
                {
                    barkCollider.enabled = enabledOnBark;
                    inBarkRange = false; //Reset flag
                    foreach (SpriteRenderer childRender in spriteRenderers)
                    {
                        childRender.color = (barkCollider.enabled) ? solidColor : transparentColor;
                    } 
                }
            }else if(barkCollider.enabled == enabledOnBark)
            {
                barkCollider.enabled = !enabledOnBark; 
                foreach (SpriteRenderer childRender in spriteRenderers)
                {
                    childRender.color = (barkCollider.enabled) ? solidColor : transparentColor;
                }
            }
        } 
    } 
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.isTrigger)
        { 
            collisionCount++;
        }  
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (!col.isTrigger)
        {
            collisionCount--;
        } 
    }
}
