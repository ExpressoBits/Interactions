using UnityEngine;

namespace ExpressoBits.Interactions
{
    public interface ISelectionResponse
    {
        void OnSelect(Transform selection);
        void OnDeselect(Transform selection);
    }
}