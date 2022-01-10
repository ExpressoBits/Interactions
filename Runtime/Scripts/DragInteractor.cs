using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace ExpressoBits.Interactions
{
    public class DragInteractor : NetworkBehaviour
    {

        private float pickupDistance;
        private Interactable draggableObject;
        [Header("Pick Up Objects")]
        [SerializeField]
        private float pickedUpDrag = 20f;
        [SerializeField]
        private float pickupHoldForce = 20f;

        private Interactor interactor;
        [SerializeField] private RayCastBasedInteractionsSelector rayCastBasedInteractionsSelector;
        private ulong lastOwnership = 0;

        private void Awake()
        {
            interactor = GetComponent<Interactor>();
        }

        private void Update()
        {
            if (!IsOwner) return;
            UpdatePickedUpObject();
        }

        [ServerRpc]
        private void ChangeOwnershipServerRpc(NetworkBehaviourReference interactableReference, ulong newClientID)
        {
            if (!interactableReference.TryGet(out Interactable interactable)) return;
            lastOwnership = interactable.NetworkObject.OwnerClientId;
            interactable.NetworkObject.ChangeOwnership(newClientID);
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { newClientID }
                }
            };
            DragClientRpc(interactable, clientRpcParams);
        }

        [ClientRpc]
        private void DragClientRpc(NetworkBehaviourReference interactableReference, ClientRpcParams clientRpcParams = default)
        {
            if (!interactableReference.TryGet(out Interactable interactable)) return;
            Rigidbody rigidbody = interactable.DraggableRigidbody;
            pickupDistance = Mathf.Clamp(rayCastBasedInteractionsSelector.LastHit.distance, 0.5f, 1f);
            draggableObject = interactable;
            rigidbody.useGravity = false;
            rigidbody.drag = pickedUpDrag;
            rigidbody.angularDrag = pickedUpDrag;
            interactable.OnDrag?.Invoke();
            interactor.ClearSelection();
            interactor.enabled = false;
        }

        [ServerRpc]
        private void RestoreOwnershipServerRpc(NetworkBehaviourReference interactableReference, Vector3 force, Vector3 velocity)
        {
            if (!interactableReference.TryGet(out Interactable interactable)) return;
            // interactable.NetworkObject.ChangeOwnership(lastOwnership);
            // interactable.DraggableRigidbody.useGravity = true;
            // interactable.DraggableRigidbody.drag = 0;
            // interactable.DraggableRigidbody.angularDrag = 0.05f;
            // StartCoroutine(DelayToAddForce(interactable,velocity));
        }


        private IEnumerator DelayToAddForce(Interactable interactable,Vector3 velocity)
        {
            yield return null;
            interactable.DraggableRigidbody.velocity = velocity;
        }

        public void Pickup()
        {
            if (interactor.HasSelection)
            {
                if (interactor.CurrentSelection != null)
                {
                    if (interactor.CurrentSelection.IsDraggable)
                    {
                        ChangeOwnershipServerRpc(interactor.CurrentSelection, OwnerClientId);
                    }
                }
            }

        }

        public void Release()
        {
            interactor.enabled = true;
            if (draggableObject != null)
            {
                Interactable dragObject = draggableObject;
                Vector3 velocity = dragObject.DraggableRigidbody.velocity;
                Drop();
                RestoreOwnershipServerRpc(dragObject,rayCastBasedInteractionsSelector.CameraTransform.forward * 20f,velocity);
            }

        }

        private void Drop()
        {
            if (draggableObject != null)
            {
                draggableObject.DraggableRigidbody.useGravity = true;
                draggableObject.DraggableRigidbody.drag = 0;
                draggableObject.DraggableRigidbody.angularDrag = 0.05f;
                draggableObject.OnDrop?.Invoke();
                draggableObject = null;
            }
        }

        private void UpdatePickedUpObject()
        {
            if (draggableObject != null)
            {
                Vector3 target = rayCastBasedInteractionsSelector.CameraTransform.position + rayCastBasedInteractionsSelector.CameraTransform.forward * pickupDistance;
                Vector3 dir = target - draggableObject.DraggableRigidbody.position;

                draggableObject.DraggableRigidbody.AddForce(dir * pickupHoldForce, ForceMode.VelocityChange);
            }
            else
            {
                if (!interactor.enabled) interactor.enabled = true;
            }
        }
    }
}

