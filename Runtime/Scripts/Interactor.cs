using UnityEngine;

namespace ExpressoBits.Interactions
{
    public class Interactor : MonoBehaviour
    {

        private Transform currentSelection;
        private IRayProvider rayProvider;
        private ISelector selector;
        private ISelectionResponse[] responses;
        
        public delegate void NewSelectionEvent(Transform newSelection);

        public NewSelectionEvent newSelectionEvent;

        private void Awake()
        {
            if(TryGetComponent(out IRayProvider rayProvider))
            {
                this.rayProvider = rayProvider;
            }
            if(TryGetComponent(out ISelector selector))
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
                if (currentSelection != null)
                    foreach(var response in responses) response.OnDeselect(currentSelection);
                if (selection != null)
                    foreach(var response in responses) response.OnSelect(selection);

                newSelectionEvent?.Invoke(selection);
                currentSelection = selection;
            }
        }

        private bool IsNewSelection(Transform selection)
        {
            return currentSelection != selection;
        }

    }
}
