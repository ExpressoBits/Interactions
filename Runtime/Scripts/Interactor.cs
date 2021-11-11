using System;
using UnityEngine;

namespace ExpressoBits.Interactions
{
    public class Interactor : MonoBehaviour
    {

        private IInteractable currentSelection;
        private IRayProvider rayProvider;
        private ISelector selector;
        private ISelectionResponse[] responses;
        private bool requestInteraction;

        public delegate void NewSelectionEvent(IInteractable newSelection);
        public delegate void PreviewEvent(string message);

        public NewSelectionEvent OnNewSelectionEvent;
        public static NewSelectionEvent OnAnyNewSelectionEvent;
        public static PreviewEvent OnAnyPreviewEvent;

        public Action<IInteractable> OnInteract;

        [SerializeField] private string defaultPreviewMessage = "for Interact";

        private void Awake()
        {
            if (TryGetComponent(out IRayProvider rayProvider))
            {
                this.rayProvider = rayProvider;
            }
            if (TryGetComponent(out ISelector selector))
            {
                this.selector = selector;
            }
            responses = GetComponents<ISelectionResponse>();
        }

        private void Update()
        {
            if (requestInteraction)
            {
                InteractWithObject();
                UpdateSelection(null);
            }
            else
            {
                selector.Check(rayProvider.CreateRay());
                var selection = selector.Selection;
                if (IsNewSelection(selection))
                {
                    UpdateSelection(selection);
                }
            }
            requestInteraction = false;
        }

        private void UpdateSelection(IInteractable selection)
        {
            if (currentSelection != null && currentSelection.Transform && currentSelection.Transform != null)
                foreach (var response in responses) response.OnDeselect(currentSelection);
            if (selection != null && selection.Transform && selection.Transform != null)
                foreach (var response in responses) response.OnSelect(selection);

            if (selection != null && selection.Transform.TryGetComponent(out IPreviewInteract preview))
            {
                OnAnyPreviewEvent?.Invoke(preview.PreviewMessage());
            }
            else
            {
                OnAnyPreviewEvent?.Invoke(defaultPreviewMessage);
            }
            if (selection != null) OnNewSelectionEvent?.Invoke(selection);
            OnAnyNewSelectionEvent?.Invoke(selection);
            currentSelection = selection;
        }

        private void InteractWithObject()
        {
            if (currentSelection != null && currentSelection.Transform && currentSelection.Transform != null)
            {
                OnInteract?.Invoke(currentSelection);
                currentSelection?.Interact();
                currentSelection = null;
            }
            else
            {
                currentSelection = null;
            }
        }

        
        public void Interact()
        {
            requestInteraction = true;
        }

        private bool IsNewSelection(IInteractable selection)
        {
            return currentSelection != selection;
        }
    }
}
