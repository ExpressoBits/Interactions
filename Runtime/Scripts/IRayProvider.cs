using UnityEngine;

namespace ExpressoBits.Interactions
{
    public interface IRayProvider
    {
        Ray CreateRay();
    }
}