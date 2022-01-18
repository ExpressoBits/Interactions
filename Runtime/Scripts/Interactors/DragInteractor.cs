using System;
using UnityEngine;

namespace ExpressoBits.Interactions
{
    public class DragInteractor : ActionInteractor
    {

        private float pickupDistance;
        private Interactable draggableObject;
        private Vector3 lastPosition;
        [SerializeField] private bool dragOnInteract = true;
        [SerializeField] private bool smoothCamera = true;

        [SerializeField] private ActionType dragActionType;

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
        public bool IsDraggable => interactor.HasSelection && interactor.CurrentSelection != null && interactor.CurrentSelection.IsDraggable;
        
        #region ActionInteractor
        public override string GetKeyMessage(Interactable interactable, ActionType actionType)
        {
            if (actionType == dragActionType) return "[HOLD MOUSE RIGHT]";
            return string.Empty;
        }

        public override void Interact(ActionType actionType, Interactable interactable)
        {
            if (actionType == dragActionType)
            {
                if (interactable.IsDraggable)
                {
                    OnDrag?.Invoke(interactable);
                    interactable.OnDrag?.Invoke();
                    if(dragOnInteract) Drag(interactable);
                    return;
                }
            }
        }

        public override bool TryTriggerAction(Interactable interactable, out ActionType actionType)
        {
            actionType = null;
            if (Input.GetMouseButtonDown(1))
            {
                if (interactable.IsDraggable)
                {
                    actionType = dragActionType;
                    return true;
                }
            }
            return false;
        }
        #endregion

        private void Update()
        {
            if (draggableObject != null)
            {
                if (Input.GetMouseButtonUp(1))
                {
                    Drop();
                    return;
                }
                Vector3 target = rayCastBasedInteractionsSelector.CameraTransform.position + rayCastBasedInteractionsSelector.CameraTransform.forward * pickupDistance;
                Vector3 dir = target - draggableObject.DraggableRigidbody.position;

                draggableObject.DraggableRigidbody.AddForce(dir * pickupHoldForce, ForceMode.VelocityChange);
                if(smoothCamera)
                {
                    Vector3 diff = rayCastBasedInteractionsSelector.CameraTransform.position - lastPosition;
                    draggableObject.transform.position += diff;
                    lastPosition = rayCastBasedInteractionsSelector.CameraTransform.position;
                }
                
            }
            else
            {
                if (!interactor.enabled) interactor.enabled = true;
            }
        }

        public void Drag(Interactable interactable)
        {
            if(!Input.GetMouseButtonDown(1)) return;
            draggableObject = interactable;
            
            pickupDistance = Mathf.Clamp(rayCastBasedInteractionsSelector.LastHit.distance, minPickupDistance, maxPickupDistance);

            if(smoothCamera)
            {
                lastPosition = rayCastBasedInteractionsSelector.CameraTransform.position;
            }

            Vector3 target = rayCastBasedInteractionsSelector.CameraTransform.position + rayCastBasedInteractionsSelector.CameraTransform.forward * pickupDistance;
            draggableObject.transform.position = target;

            interactor.ClearSelection();
            interactor.enabled = false;

            Dragged(draggableObject);
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
            
            rigidbody.useGravity = false;
            rigidbody.drag = pickedUpDrag;
            rigidbody.angularDrag = pickedUpDrag;
            rigidbody.position = transform.position;
        }

        private void Droped(Interactable interactable)
        {
            Rigidbody rigidbody = interactable.DraggableRigidbody;

            rigidbody.useGravity = true;
            rigidbody.drag = 0;
            rigidbody.angularDrag = 0.05f;
        }

        public void SetDragOnInteract(bool dragOnInteract)
        {
            this.dragOnInteract = dragOnInteract;
        }
    }
}

