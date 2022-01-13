using System;
using UnityEngine;

namespace ExpressoBits.Interactions
{
    public class DragInteractor : MonoBehaviour
    {
        private float pickupDistance;
        private Interactable draggableObject;
        private Interactor interactor;
        private Vector3 lastPosition;

        [Header("Pick Up Objects")]
        [SerializeField]
        private float pickedUpDrag = 20f;
        [SerializeField]
        private float pickupHoldForce = 20f;

        [SerializeField]
        private float minPickupDistance = 1f;
        [SerializeField]
        private float maxPickupDistance = 2f;

        [Header("Selector")]
        [SerializeField] private RayCastBasedInteractionsSelector rayCastBasedInteractionsSelector;

        public Action<Interactable> OnDrag;
        public Action<Interactable> OnDrop;
        public Interactor Interactor => interactor;
        public bool IsDraggable => interactor.HasSelection && interactor.CurrentSelection != null && interactor.CurrentSelection.IsDraggable;

        private void Awake()
        {
            interactor = GetComponent<Interactor>();
        }

        private void Update()
        {
            if (!enabled) return;
            if (draggableObject != null)
            {
                Vector3 target = rayCastBasedInteractionsSelector.CameraTransform.position + rayCastBasedInteractionsSelector.CameraTransform.forward * pickupDistance;

                Vector3 dir = target - draggableObject.DraggableRigidbody.position;

                draggableObject.DraggableRigidbody.AddForce(dir * pickupHoldForce, ForceMode.VelocityChange);
                draggableObject.transform.position += rayCastBasedInteractionsSelector.CameraTransform.position - lastPosition;
                lastPosition = rayCastBasedInteractionsSelector.CameraTransform.position;
            }
            else
            {
                if (!interactor.enabled) interactor.enabled = true;
            }
        }

        public void Drag()
        {
            if(IsDraggable)
            {
                draggableObject = interactor.CurrentSelection;
                OnDrag?.Invoke(draggableObject);
                draggableObject.OnDrag?.Invoke();
                Dragged(draggableObject);

                lastPosition = rayCastBasedInteractionsSelector.CameraTransform.position;

                Vector3 target = rayCastBasedInteractionsSelector.CameraTransform.position + rayCastBasedInteractionsSelector.CameraTransform.forward * pickupDistance;
                draggableObject.transform.position = target;

                interactor.ClearSelection();
                interactor.enabled = false;
            }
        }

        public void Drop()
        {
            interactor.enabled = true;
            if (draggableObject != null)
            {
                OnDrop?.Invoke(draggableObject);
                draggableObject.OnDrop?.Invoke();
                Droped(draggableObject);
                draggableObject = null;
            }
        }

        private void Dragged(Interactable interactable)
        {
            Rigidbody rigidbody = interactable.DraggableRigidbody;
            pickupDistance = Mathf.Clamp(rayCastBasedInteractionsSelector.LastHit.distance, minPickupDistance, maxPickupDistance);
            
            rigidbody.useGravity = false;
            rigidbody.drag = pickedUpDrag;
            rigidbody.angularDrag = pickedUpDrag;
        }

        private void Droped(Interactable interactable)
        {
            interactable.DraggableRigidbody.useGravity = true;
            interactable.DraggableRigidbody.drag = 0;
            interactable.DraggableRigidbody.angularDrag = 0.05f;
        }
    }
}

