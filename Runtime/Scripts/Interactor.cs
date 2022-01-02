using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace ExpressoBits.Interactions
{
    /// <summary>
    /// Responsible for interacting with interactables, stores the interactable current selected with CurrentSelection
    /// </summary>
    public class Interactor : NetworkBehaviour
    {

        public delegate void NewSelectionEvent(Interactable newSelection);
        public delegate void PreviewEvent(string message);

        public NewSelectionEvent OnNewSelectionEvent;
        public static NewSelectionEvent OnAnyNewSelectionEvent;
        public static PreviewEvent OnAnyPreviewEvent;

        public Action<Interactable> OnInteract;
        public Action<Interactable> OnMoreOptions;
        public Action OnCancelMoreOptionsInteract;

        private Interactable currentSelection;
        private ISelectionResponse[] responses;
        private bool requestInteraction;
        private bool requestMoreOptions;
        private bool hasSelection;
        [SerializeField] private string defaultPreviewMessage = "for Interact";
        [SerializeField] private Selector selector;
        [SerializeField] private SelectionResponse[] selectionResponses;
        [SerializeField] private Dictionary<ActionType, IActionInteractor> interactors = new Dictionary<ActionType, IActionInteractor>();

        /// <summary>
        /// Something was selected by ISelector
        /// </summary>
        public bool HasSelection => hasSelection;
        public Interactable CurrentSelection => currentSelection;

        private void Awake()
        {
            responses = GetComponents<ISelectionResponse>();
        }

        private void Update()
        {
            selector.Check();
            var selection = selector.Selection;
            if (IsNewSelection(selection))
            {
                if(selection == null)
                {
                    UpdateSelection(null);
                }
                else if(selection.TryGetComponent(out Interactable interactable))
                {
                    UpdateSelection(interactable);
                }
            }

            CheckForSelection(currentSelection);

            if (requestMoreOptions)
            {
                if (currentSelection && currentSelection != null)
                {
                    MoreOptions(currentSelection);
                }
            }

            if (requestInteraction)
            {
                if (currentSelection && currentSelection != null)
                {
                    InteractServerRpc(currentSelection);
                }
            }

            requestInteraction = false;
            requestMoreOptions = false;
        }
        

        private void MoreOptions(Interactable interactable)
        {
            OnMoreOptions?.Invoke(interactable);
        }

        #region Server Commands
        [ServerRpc]
        public void InteractServerRpc(NetworkBehaviourReference interactableReference)
        {
            if (interactableReference.TryGet(out Interactable interactable))
            {
                InteractWithAction(interactable,interactable.DefaultAction);
            }
        }

        [ServerRpc]
        public void InteractServerRpc(NetworkBehaviourReference interactableReference, int actionIndex)
        {
            if (interactableReference.TryGet(out Interactable interactable))
            {
                InteractWithAction(interactable,interactable.Actions[actionIndex]);
            }
        }

        public void InteractWithAction(Interactable interactable, InteractableAction action)
        {
            OnInteract?.Invoke(interactable);
            ActionInteract(interactable, action.ActionType);
        }

        public void ActionInteract(Interactable interactable, ActionType actionType)
        {
            if (interactors.TryGetValue(actionType, out IActionInteractor interactor))
            {
                interactor.Interact(actionType, interactable);
            }
        }
        #endregion


        private void CheckForSelection(Interactable selection)
        {
            if (hasSelection && selection == null)
            {
                OnAnyNewSelectionEvent?.Invoke(null);
                hasSelection = false;
            }
            else
            {
                if (selection != null) hasSelection = true;
            }
        }

        private void UpdateSelection(Interactable selection)
        {
            if (currentSelection && currentSelection != null)
            {
                foreach (var response in responses) response.OnDeselect(currentSelection.transform);
                foreach (var response in selectionResponses) response.OnDeselect(currentSelection.transform);
            }

            if (selection && selection != null)
            {
                foreach (var response in responses) response.OnSelect(selection.transform);
                foreach (var response in selectionResponses) response.OnSelect(selection.transform);
            }

            if (selection != null && selection.IsShowPreviewMessage)
            {
                OnAnyPreviewEvent?.Invoke(selection.PreviewMessage());
            }
            OnNewSelectionEvent?.Invoke(selection);
            OnAnyNewSelectionEvent?.Invoke(selection);
            currentSelection = selection;
        }


        public void AddInteractor(IActionInteractor interactableAction)
        {
            foreach (var type in interactableAction.ActionTypes)
            {
                interactors.Add(type, interactableAction);
            }
        }

        public void MoreOptions()
        {
            requestMoreOptions = true;
        }

        public void CancelMoreOptions()
        {
            OnCancelMoreOptionsInteract?.Invoke();
        }

        public void Interact()
        {
            requestInteraction = true;
        }

        private bool IsNewSelection(Transform interactable)
        {
            return (currentSelection == null && interactable != null ) || (currentSelection != null && currentSelection.transform != interactable);
        }
    }
}
