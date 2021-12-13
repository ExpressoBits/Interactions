using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace ExpressoBits.Interactions
{
    public abstract class InteractableAction : NetworkBehaviour
    {
        public ActionType ActionType => actionType;
        public abstract void Action(Interactor interactor);
        public abstract string PreviewMessage { get; }

        [SerializeField] protected ActionType actionType;
    }
}