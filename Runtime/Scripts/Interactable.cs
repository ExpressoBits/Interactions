using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExpressoBits.Interactions
{
    /// <summary>
    /// An interface to elements that will be interactable by the interactor
    /// </summary>
    public class Interactable : MonoBehaviour
    {

        public bool IsShowPreviewMessage => isShowPreviewMessage;
        public InteractableAction[] InteractableActions => interactableActions;
        public List<ActionType> ActionTypes => actionTypes;
        public bool IsDraggable => isDraggable;
        public Rigidbody DraggableRigidbody => draggableRigidbody;
        public string Name => interactableName;
        public Dictionary<ActionType,InteractableAction> InteractableActionTypeDictionary
        {
            get
            {
                if(interactableActionsCache == null)
                {
                    interactableActionsCache = new Dictionary<ActionType, InteractableAction>();
                    foreach (var interactableAction in interactableActions)
                    {
                        interactableActionsCache.Add(interactableAction.ActionType, interactableAction);
                    }
                }
                return interactableActionsCache;
            }
        }

        [SerializeField] private InteractableAction[] interactableActions;
        [SerializeField] private bool isShowPreviewMessage;
        [SerializeField] private bool isDraggable;
        [SerializeField] private Rigidbody draggableRigidbody;
        [SerializeField] private List<ActionType> actionTypes;
        [SerializeField] private string interactableName;

        private Dictionary<ActionType, InteractableAction> interactableActionsCache;

        public Action OnDrag;
        public Action OnDrop;
    }
}

