using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExpressoBits.Interactions
{
    /// <summary>
    /// Responsible for interacting with interactables, stores the interactable current selected with CurrentSelection
    /// </summary>
    public class Interactor : MonoBehaviour
    {

        public delegate void NewSelectionEvent(Interactable newSelection);

        public NewSelectionEvent OnNewSelectionEvent;

        public Action<Interactable> OnInteract;

        private Interactable currentInteractable;
        private ISelectionResponse[] responses;
        private bool hasSelection;
        private Dictionary<ActionType, ActionInteractor> interactors = new Dictionary<ActionType, ActionInteractor>();
        [SerializeField] private Selector selector;
        [SerializeField] private SelectionResponse[] selectionResponses;
        [SerializeField] private ActionInteractor[] actionInteractors;

        /// <summary>
        /// Something was selected by ISelector
        /// </summary>
        public bool HasSelection => hasSelection;
        public Interactable CurrentSelection => currentInteractable;

        private void Awake()
        {
            responses = GetComponents<ISelectionResponse>();
            foreach (var actionInteractor in actionInteractors)
            {
                AddActionInteractor(actionInteractor);
            }
        }

        private void Update()
        {
            selector.Check();
            var selection = selector.Selection;
            if (IsNewSelection(selection))
            {
                if (selection == null)
                {
                    UpdateSelection(null);
                }
                else if (selection.TryGetComponent(out Interactable interactable))
                {
                    UpdateSelection(interactable);
                }
            }

            CheckForSelection(currentInteractable);
        }

        public void ClearSelection()
        {
            UpdateSelection(null);
            OnNewSelectionEvent?.Invoke(null);
            hasSelection = false;
        }

        public void InteractWithAction(Interactable interactable, ActionType actionType)
        {
            OnInteract?.Invoke(interactable);
            ActionInteract(interactable, actionType);
        }

        public void InteractWithAction(Interactable interactable, int actionTypeIndex)
        {
            InteractWithAction(interactable, interactable.ActionTypes[actionTypeIndex]);
        }

        public void InteractWithAction(Interactable interactable, InteractableAction interactableAction)
        {
            InteractWithAction(interactable, interactableAction.ActionType);
        }

        public void ActionInteract(Interactable interactable, ActionType actionType)
        {
            if (interactors.TryGetValue(actionType, out ActionInteractor interactor))
            {
                interactor.Interact(actionType, interactable);
            }
        }

        private void CheckForSelection(Interactable selection)
        {
            if (hasSelection && selection == null)
            {
                OnNewSelectionEvent?.Invoke(null);
                hasSelection = false;
            }
            else
            {
                if (selection != null) hasSelection = true;
            }
        }

        private void UpdateSelection(Interactable selection)
        {
            if (currentInteractable && currentInteractable != null)
            {
                foreach (var response in responses) response.OnDeselect(currentInteractable.transform);
                foreach (var response in selectionResponses) response.OnDeselect(currentInteractable.transform);
            }

            if (selection && selection != null)
            {
                foreach (var response in responses) response.OnSelect(selection.transform);
                foreach (var response in selectionResponses) response.OnSelect(selection.transform);
            }

            currentInteractable = selection;
            OnNewSelectionEvent?.Invoke(selection);
        }

        public List<PreviewMessage> GetPreviewMessages(Interactable interactable)
        {
            List<PreviewMessage> previewMessages = new List<PreviewMessage>(interactable.ActionTypes.Count);
            if (interactable.IsShowPreviewMessage)
            {
                foreach (var action in interactable.ActionTypes)
                {
                    previewMessages.Add(new PreviewMessage() { actionType = action, message = action.DefaultPreviewMessage });
                }
                for (int i = 0; i < previewMessages.Count; i++)
                {
                    ActionType actionType = previewMessages[i].actionType;
                    if (interactors.TryGetValue(actionType, out ActionInteractor actionInteractor))
                    {
                        PreviewMessage previewMessage = previewMessages[i];
                        previewMessage.message = actionInteractor.GetKeyMessage(interactable, actionType) + " " + previewMessages[i].message;
                        previewMessages[i] = previewMessage;
                    }
                }
            }
            return previewMessages;
        }


        public void AddActionInteractor(ActionInteractor interactableAction)
        {
            interactableAction.SetInteractor(this);
            foreach (var type in interactableAction.ActionTypes)
            {
                interactors.Add(type, interactableAction);
            }
        }

        public void TryInteract()
        {
            if (TryGetActionType(out ActionType actionType))
            {
                InteractWithAction(CurrentSelection, actionType);
            }
        }

        public bool TryGetActionType(out ActionType actionType)
        {
            actionType = null;
            if (!CurrentSelection || CurrentSelection == null) return false;
            foreach (var actionInteractor in actionInteractors)
            {
                if (actionInteractor.TryTriggerAction(CurrentSelection, out ActionType interactorActionType))
                {
                    actionType = interactorActionType;
                    return true;
                }
            }
            return false;
        }

        private bool IsNewSelection(Transform interactable)
        {
            return (currentInteractable == null && interactable != null) || (currentInteractable != null && currentInteractable.transform != interactable);
        }
    }
}
