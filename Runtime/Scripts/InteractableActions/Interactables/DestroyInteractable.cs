using UnityEngine;

namespace ExpressoBits.Interactions.Interactables
{
    public class DestroyInteractable : InteractableAction
    {
        private const string PreviewMessageString = "for Destroy Object";

        public GameObject[] instantiateInDestroy;

        public Transform Transform => transform;

        public void SpawnParticles()
        {
            foreach(GameObject go in instantiateInDestroy)
            {
                Instantiate(go,transform.position,transform.rotation);
            }
        }

        public override string PreviewMessage => PreviewMessageString;

        public override void Action(Interactor interactor)
        {
            SpawnParticles();
            Destroy(gameObject);
        }
    }
}

