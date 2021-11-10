using System;
using UnityEngine;

namespace ExpressoBits.Interactions
{
    public class Interactor : MonoBehaviour
    {

        private Transform currentSelection;
        private IInteractable interactable;
        private IRayProvider rayProvider;
        private ISelector selector;
        private ISelectionResponse[] responses;

        public delegate void NewSelectionEvent(Transform newSelection);
        public delegate void PreviewEvent(string message);

        public NewSelectionEvent OnNewSelectionEvent;
        public static NewSelectionEvent OnAnyNewSelectionEvent;
        public static PreviewEvent OnAnyPreviewEvent;

        public Action<Transform> OnInteract;

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
            selector.Check(rayProvider.CreateRay());
            var selection = selector.Selection;
            if (IsNewSelection(selection))
            {
                UpdateSelection(selection);
            }
        }

        private void UpdateSelection(Transform selection)
        {
            if (currentSelection != null)
                foreach (var response in responses) response.OnDeselect(currentSelection);
            if (selection != null)
                foreach (var response in responses) response.OnSelect(selection);

            if (selection != null && selection.TryGetComponent(out IInteractable interactable))
            {
                this.interactable = interactable;
                if (selection.TryGetComponent(out IPreviewInteract preview))
                {
                    OnAnyPreviewEvent?.Invoke(preview.PreviewMessage());
                }
                else
                {
                    OnAnyPreviewEvent?.Invoke(defaultPreviewMessage);
                }
            }
            else
            {
                this.interactable = null;
            }
            OnNewSelectionEvent?.Invoke(selection);
            OnAnyNewSelectionEvent?.Invoke(selection);
            currentSelection = selection;
        }

        private void InteractWithObject()
        {
            if (interactable != null)
            {
                OnInteract?.Invoke(interactable.Transform);
                interactable?.Interact();
            }
        }

        public void Interact()
        {
            InteractWithObject();
            UpdateSelection(null);
        }

        private bool IsNewSelection(Transform selection)
        {
            return currentSelection != selection;
        }

    }
}
