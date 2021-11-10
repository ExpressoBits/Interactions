using UnityEngine;

namespace ExpressoBits.Interactions.Interactables
{
    public class DestroyInteractable : MonoBehaviour, IInteractable, IPreviewInteract
    {
        private const string PriviewMessage = "for Destroy Object";

        public GameObject[] instantiateInDestroy;

        public Transform Transform => transform;

        public void Interact()
        {
            Destroy(gameObject);
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

