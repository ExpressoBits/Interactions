using Unity.Netcode;
using UnityEngine;

namespace ExpressoBits.Interactions
{
    public abstract class Selector : ScriptableObject
    {
        public bool IsInteractable => isInteractable;
        public Interactable Selection => selection;
        public abstract void Check(Ray ray, float additionalDistance = 0f);

        protected Interactable selection;
        protected bool isInteractable;
    }
}