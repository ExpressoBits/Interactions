using System;
using UnityEngine;

namespace ExpressoBits.Interactions
{
    /// <summary>
    /// An interface to elements that will be interactable by the interactor
    /// </summary>
    public class Interactable : MonoBehaviour
    {
        
        public bool IsShowPreviewMessage => isShowPreviewMessage;
        public InteractableAction DefaultAction => defaultAction;
        public InteractableAction[] Actions => actions;
        public bool IsDraggable => isDraggable;
        public Rigidbody DraggableRigidbody => draggableRigidbody;

        [SerializeField] private InteractableAction defaultAction;
        [SerializeField] private InteractableAction[] actions;

        [SerializeField] private bool isShowPreviewMessage;
        [SerializeField] private bool isDraggable;
        [SerializeField] private Rigidbody draggableRigidbody;

        public Action OnDrag;
        public Action OnDrop;

        public void DefaultInteract(Interactor interactor)
        {
            defaultAction.Action(interactor);
        }

        public string PreviewMessage()
        {
            return DefaultAction.PreviewMessage;
        }
    }
}

