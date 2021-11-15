using Unity.Netcode;
using UnityEngine;

namespace ExpressoBits.Interactions.Interactables
{
    public class DestroyInteractable : NetworkBehaviour, IInteractable, IPreviewInteract
    {
        private const string PriviewMessage = "for Destroy Object";

        public GameObject[] instantiateInDestroy;

        public Transform Transform => transform;

        public void Interact(Interactor interactor)
        {
            SpawnParticlesClientRpc();
            NetworkObject.Despawn(gameObject);
        }

        [ClientRpc]
        public void SpawnParticlesClientRpc()
        {
            foreach(GameObject go in instantiateInDestroy)
            {
                Instantiate(go,transform.position,transform.rotation);
            }
        }

        public string PreviewMessage()
        {
            return PriviewMessage;
        }
    }
}

