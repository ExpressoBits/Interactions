using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace ExpressoBits.Interactions
{
    public class Interactor : NetworkBehaviour
    {

        private Interactable currentSelection;
        private IRayProvider rayProvider;
        private ISelectionResponse[] responses;
        private bool requestInteraction;
        private bool hasSelection;

        public delegate void NewSelectionEvent(Interactable newSelection);
        public delegate void PreviewEvent(string message);

        public NewSelectionEvent OnNewSelectionEvent;
        public static NewSelectionEvent OnAnyNewSelectionEvent;
        public static PreviewEvent OnAnyPreviewEvent;

        public Action<Interactable> OnInteract;

        [SerializeField] private string defaultPreviewMessage = "for Interact";
        [SerializeField] private Selector selector;
        [SerializeField] private SelectionResponse[] selectionResponses;
        [SerializeField] private float additionalDistanceToSelector = 0f;

        [SerializeField] private Dictionary<ActionType, Interactions.IActionInteractor> interactors = new Dictionary<ActionType, IActionInteractor>();

        public bool HasSelection => hasSelection;

        private void Awake()
        {
            if (TryGetComponent(out IRayProvider rayProvider))
            {
                this.rayProvider = rayProvider;
            }
            responses = GetComponents<ISelectionResponse>();
        }

        private void Update()
        {
            selector.Check(rayProvider.CreateRay(), additionalDistanceToSelector);
            var selection = selector.Selection;
            if (IsNewSelection(selection))
            {
                UpdateSelection(selection);
            }

            CheckForSelection(selection);

            if (requestInteraction)
            {
                if (currentSelection && currentSelection != null)
                {
                    InteractServerRpc(currentSelection.NetworkObject);
                }
            }

            requestInteraction = false;
        }

        #region Server Commands
        [ServerRpc]
        public void InteractServerRpc(NetworkObjectReference targetReference)
        {
            if (targetReference.TryGet(out NetworkObject targetObject))
            {
                if (targetObject && targetObject != null)
                {
                    if (targetObject.transform.TryGetComponent(out Interactable interactable))
                    {
                        Interact(interactable);
                    }
                }
            }

        }

        public void Interact(Interactable interactable)
        {
            OnInteract?.Invoke(interactable);
            ActionInteract(interactable, interactable.DefaultAction.ActionType);
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


        public void Interact()
        {
            requestInteraction = true;
        }

        private bool IsNewSelection(Interactable interactable)
        {
            return currentSelection != interactable;
        }

        public void SetAdditionalDistanceForSelector(float additionalDistance)
        {
            this.additionalDistanceToSelector = additionalDistance;
        }
    }
}
