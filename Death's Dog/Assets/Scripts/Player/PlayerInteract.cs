using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixeye.Unity;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerInteract : MonoBehaviour
{
    #region Components
    [Foldout("Components", true)] 
    [SerializeField] private PlayerInput input;
    PlayerMovement pMove;
    #endregion 

    #region Interact Variables
    [Foldout("Interact", true)] 
    [SerializeField] private Collider2D interactCheck;
    [SerializeField] private LayerMask interactLayer; //Layer of ground 
    private RaycastHit2D[] hits;
    private Interactable currentInteractable; 
    #endregion 

    private void Awake()
    {
        pMove = GetComponent<PlayerMovement>();
    } 
     
    void Update()
    {
        if (pMove.canInput && !VariableManager.Instance.inDialogue)
        {
            Interacting();
        }
        else if(currentInteractable != null)
        {
            ResetCurrentInteractable();
        }
    }

    #region Interact
    private void Interacting()
    { 
        hits = Physics2D.BoxCastAll(interactCheck.bounds.center, interactCheck.bounds.size, 0, interactCheck.transform.forward, 0, interactLayer); //Box to check for an item
        hits = hits.OrderBy(x => Vector2.Distance(this.transform.position, x.transform.position)).ToArray();

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.TryGetComponent(out Interactable interact))
            {
                if (input.currentActionMap.FindAction("Interact").GetButtonDown())
                {
                    interact.InteractWith(this);
                }
                else if(currentInteractable != interact)
                {
                    if (currentInteractable != null)
                    {
                        ResetCurrentInteractable();
                    }
                    interact.Nearby();
                    currentInteractable = interact;
                }
                return;
            }
            else
            {
                Debug.LogError("Missing interactable component");
            }
        }
        if(hits.Length == 0 && currentInteractable != null)
        {
            ResetCurrentInteractable();
        } 
    }
    private void ResetCurrentInteractable()
    {
        currentInteractable.Away();
        currentInteractable = null;
    }
    #endregion

}
