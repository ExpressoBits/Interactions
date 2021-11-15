using System;
using Unity.Netcode;
using UnityEngine;

namespace ExpressoBits.Interactions
{
    public class Interactor : NetworkBehaviour
    {

        private NetworkObject currentSelection;
        private IRayProvider rayProvider;
        private ISelectionResponse[] responses;
        private bool requestInteraction;
        private bool hasSelection;

        public delegate void NewSelectionEvent(NetworkObject newSelection);
        public delegate void PreviewEvent(string message);

        public NewSelectionEvent OnNewSelectionEvent;
        public static NewSelectionEvent OnAnyNewSelectionEvent;
        public static PreviewEvent OnAnyPreviewEvent;

        public Action<NetworkObject> OnInteract;

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
                if (currentSelection && currentSelection != null)
                {
                    InteractServerRpc(currentSelection);
                }
            }
            
            requestInteraction = false;
        }

        #region Server Commands
        [ServerRpc]
        public void InteractServerRpc(NetworkObjectReference targetReference)
        {
            if(targetReference.TryGet(out NetworkObject targetObject))
            {
                if (targetObject && targetObject != null)
                {
                    OnInteract?.Invoke(targetObject);
                    if(targetObject.transform.TryGetComponent(out IInteractable interactable))
                    {
                        interactable.Interact(this);
                    }
                }
            }
            
        }
        #endregion

        private void CheckForSelection(NetworkObject selection)
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

        private void UpdateSelection(NetworkObject selection)
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

        
        public void Interact()
        {
            requestInteraction = true;
        }

        private bool IsNewSelection(NetworkObject networkObject)
        {
            return currentSelection != networkObject;
        }

        public void SetAdditionalDistanceForSelector(float additionalDistance)
        {
            this.additionalDistanceToSelector = additionalDistance;
        }
    }
}
