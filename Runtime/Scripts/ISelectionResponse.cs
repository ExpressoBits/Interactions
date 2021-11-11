using UnityEngine;

namespace ExpressoBits.Interactions
{
    public interface ISelectionResponse
    {
        void OnSelect(IInteractable selection);
        void OnDeselect(IInteractable selection);
    }
}