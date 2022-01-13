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
        public delegate void PreviewEvent(string message);

        public NewSelectionEvent OnNewSelectionEvent;
        public static NewSelectionEvent OnAnyNewSelectionEvent;
        public static PreviewEvent OnAnyPreviewEvent;

        public Action<Interactable> OnInteract;
        public Action<Interactable> OnMoreOptions;
        public Action OnCancelMoreOptionsInteract;

        private Interactable currentSelection;
        private ISelectionResponse[] responses;
        private bool requestMoreOptions;
        private bool hasSelection;
        private bool checkNewSelection = true;
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
            if(!checkNewSelection) return;
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

            CheckForSelection(currentSelection);

            if (requestMoreOptions)
            {
                if (currentSelection && currentSelection != null)
                {
                    MoreOptions(currentSelection);
                }
            }

            requestMoreOptions = false;
        }

        public void ClearSelection()
        {
            UpdateSelection(null);
            OnAnyNewSelectionEvent?.Invoke(null);
            hasSelection = false;
        }

        private void MoreOptions(Interactable interactable)
        {
            OnMoreOptions?.Invoke(interactable);
        }

        public void Interact(Interactable interactable)
        {
            InteractWithAction(interactable, interactable.DefaultAction);
        }

        public void Interact(Interactable interactable, int actionIndex)
        {
            InteractWithAction(interactable, interactable.Actions[actionIndex]);
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
            if (!enabled) return;
            if (currentSelection && currentSelection != null)
            {
                Interact(currentSelection);
            }
        }

        private bool IsNewSelection(Transform interactable)
        {
            return (currentSelection == null && interactable != null) || (currentSelection != null && currentSelection.transform != interactable);
        }

        public void SetCheckNewSelection(bool checkNewSelection)
        {
            this.checkNewSelection = checkNewSelection;
        }
    }
}
