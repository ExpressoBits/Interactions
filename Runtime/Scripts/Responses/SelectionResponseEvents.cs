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

        public void OnSelect(Transform selection)
        {
            OnSelectEvent?.Invoke(selection);
            OnSelectUnityEvent.Invoke(selection);
        }

        public void OnDeselect(Transform selection)
        {
            OnDeselectEvent?.Invoke(selection);
            OnDeselectUnityEvent.Invoke(selection);
        }

    }
}

