using System;
using UnityEngine;

namespace ExpressoBits.Interactions
{
    public class Interactor : MonoBehaviour
    {

        private Transform currentSelection;
        private IRayProvider rayProvider;
        private ISelectionResponse[] responses;
        private bool requestInteraction;
        private bool hasSelection;

        public delegate void NewSelectionEvent(Transform newSelection);
        public delegate void PreviewEvent(string message);

        public NewSelectionEvent OnNewSelectionEvent;
        public static NewSelectionEvent OnAnyNewSelectionEvent;
        public static PreviewEvent OnAnyPreviewEvent;

        public Action<Transform> OnInteract;

        [SerializeField] private string defaultPreviewMessage = "for Interact";
        [SerializeField] private Selector selector;
        [SerializeField] private SelectionResponse[] selectionResponses;
        [SerializeField] private float additionalDistanceToSelector = 0f;

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
            selector.Check(rayProvider.CreateRay(),additionalDistanceToSelector);
            var selection = selector.Selection;
            if (IsNewSelection(selection))
            {
                UpdateSelection(selection);
            }

            CheckForSelection(selection);

            if (requestInteraction)
            {
                InteractWithObject();
            }
            
            requestInteraction = false;
        }

        private void CheckForSelection(Transform selection)
        {
            if(hasSelection && selection == null)
            {
                OnAnyNewSelectionEvent?.Invoke(null);
                hasSelection = false;
            }else
            {
                if(selection != null) hasSelection = true;
            }
        }

        private void UpdateSelection(Transform selection)
        {
            if (currentSelection && currentSelection != null)
            {
                foreach (var response in responses) response.OnDeselect(currentSelection);
                foreach (var response in selectionResponses) response.OnDeselect(currentSelection);
            }
                
            if (selection && selection != null)
            {
                foreach (var response in responses) response.OnSelect(selection);
                foreach (var response in selectionResponses) response.OnSelect(selection);
            }
            
            if (selection != null && selection.TryGetComponent(out IPreviewInteract preview))
            {
                OnAnyPreviewEvent?.Invoke(preview.PreviewMessage());
            }
            else
            {
                OnAnyPreviewEvent?.Invoke(defaultPreviewMessage);
            }
            OnNewSelectionEvent?.Invoke(selection);
            OnAnyNewSelectionEvent?.Invoke(selection);
            currentSelection = selection;
        }

        private void InteractWithObject()
        {
            if (currentSelection && currentSelection != null)
            {
                OnInteract?.Invoke(currentSelection);
                if(currentSelection.TryGetComponent(out IInteractable interactable))
                {
                    interactable.Interact();
                }
            }
        }

        
        public void Interact()
        {
            requestInteraction = true;
        }

        private bool IsNewSelection(Transform selection)
        {
            return currentSelection != selection;
        }

        public void SetAdditionalDistanceForSelector(float additionalDistance)
        {
            this.additionalDistanceToSelector = additionalDistance;
        }
    }
}
