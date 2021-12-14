using Unity.Netcode;
using UnityEngine;

namespace ExpressoBits.Interactions
{
    public abstract class Selector : MonoBehaviour
    {
        public Transform Selection => selection;
        public abstract void Check();

        protected Transform selection;
    }
}