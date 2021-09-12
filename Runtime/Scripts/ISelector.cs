using UnityEngine;

namespace ExpressoBits.Interactions
{
    public interface ISelector
    {
        Transform Selection { get; }
        void Check(Ray ray);
    }
}