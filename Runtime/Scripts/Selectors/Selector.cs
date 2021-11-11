using UnityEngine;

namespace ExpressoBits.Interactions
{
    public abstract class Selector : ScriptableObject
    {
        public IInteractable Selection => selection;
        public abstract void Check(Ray ray);

        protected IInteractable selection;
    }
}