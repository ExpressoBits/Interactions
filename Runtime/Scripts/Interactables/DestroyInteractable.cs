using UnityEngine;

namespace ExpressoBits.Interactions.Interactables
{
    public class DestroyInteractable : MonoBehaviour, IInteractable, IPreviewInteract
    {
        private const string PriviewMessage = "for Destroy Object";

        public void Interact()
        {
            Destroy(gameObject);
        }

        public string Preview()
        {
            return PriviewMessage;
        }
    }
}

