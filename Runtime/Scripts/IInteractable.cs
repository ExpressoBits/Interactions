using UnityEngine;

namespace ExpressoBits.Interactions
{
    /// <summary>
    /// An interface to elements that will be interactable by the interactor
    /// </summary>
    public interface IInteractable
    {
        Transform Transform { get; }
        void Interact();
    }
}

