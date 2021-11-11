using UnityEngine;

namespace ExpressoBits.Interactions
{
    public interface ISelector
    {
        IInteractable Selection { get; }
        void Check(Ray ray);
    }
}