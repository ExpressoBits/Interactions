using Unity.Netcode;
using UnityEngine;

namespace ExpressoBits.Interactions.Interactables
{
    public class DestroyInteractable : InteractableAction
    {
        private const string PreviewMessageString = "for Destroy Object";

        public GameObject[] instantiateInDestroy;

        public Transform Transform => transform;

        [ClientRpc]
        public void SpawnParticlesClientRpc()
        {
            foreach(GameObject go in instantiateInDestroy)
            {
                Instantiate(go,transform.position,transform.rotation);
            }
        }

        public override string PreviewMessage => PreviewMessageString;

        public override void Action(Interactor interactor)
        {
            SpawnParticlesClientRpc();
            NetworkObject.Despawn(gameObject);
        }
    }
}

