using Unity.Netcode;
using UnityEngine;

namespace ExpressoBits.Interactions
{
    public abstract class Selector : ScriptableObject
    {
        public NetworkObject Selection => selection;
        public abstract void Check(Ray ray, float additionalDistance = 0f);

        protected NetworkObject selection;
    }
}