using UnityEngine;
using UnityEngine.Events;

namespace ExpressoBits.Interactions.Responses
{
    public class SelectionResponseEvents : MonoBehaviour, ISelectionResponse
    {

        public delegate void SelectionResponseEvent(IInteractable selection);

        public SelectionResponseEvent OnSelectEvent;
        public SelectionResponseEvent OnDeselectEvent;

        public UnityEvent<IInteractable> OnSelectUnityEvent;
        public UnityEvent<IInteractable> OnDeselectUnityEvent;

        public void OnSelect(IInteractable interactable)
        {
            OnSelectEvent?.Invoke(interactable);
            OnSelectUnityEvent.Invoke(interactable);
        }

        public void OnDeselect(IInteractable interactable)
        {
            OnDeselectEvent?.Invoke(interactable);
            OnDeselectUnityEvent.Invoke(interactable);
        }

    }
}

