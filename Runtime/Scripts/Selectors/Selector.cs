using UnityEngine;

namespace ExpressoBits.Interactions
{
    public abstract class Selector : ScriptableObject
    {
        public Transform Selection => selection;
        public abstract void Check(Ray ray, float additionalDistance = 0f);

        protected Transform selection;
    }
}