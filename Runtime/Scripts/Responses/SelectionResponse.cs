using UnityEngine;

namespace ExpressoBits.Interactions
{
    public abstract class SelectionResponse : ScriptableObject, ISelectionResponse
    {
        public abstract void OnDeselect(IInteractable selection);
        public abstract void OnSelect(IInteractable selection);
    }
}

