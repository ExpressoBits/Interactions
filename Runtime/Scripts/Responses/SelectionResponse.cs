using UnityEngine;

namespace ExpressoBits.Interactions
{
    public abstract class SelectionResponse : ScriptableObject, ISelectionResponse
    {
        public abstract void OnDeselect(Transform selection);
        public abstract void OnSelect(Transform selection);
    }
}

