using UnityEngine;

namespace ExpressoBits.Interactions
{
    public interface ISelector
    {
        Transform GetSelection();
        void Check(Ray ray);
    }
}