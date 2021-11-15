using UnityEngine;
using UnityEngine.Events;

namespace ExpressoBits.Interactions.Responses
{
    public class SelectionResponseEvents : MonoBehaviour, ISelectionResponse
    {

        public delegate void SelectionResponseEvent(Transform selection);

        public SelectionResponseEvent OnSelectEvent;
        public SelectionResponseEvent OnDeselectEvent;

        public UnityEvent<Transform> OnSelectUnityEvent;
        public UnityEvent<Transform> OnDeselectUnityEvent;

        public void OnSelect(Transform interactable)
        {
            OnSelectEvent?.Invoke(interactable);
            OnSelectUnityEvent.Invoke(interactable);
        }

        public void OnDeselect(Transform interactable)
        {
            OnDeselectEvent?.Invoke(interactable);
            OnDeselectUnityEvent.Invoke(interactable);
        }

    }
}

