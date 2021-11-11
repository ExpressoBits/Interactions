using UnityEngine;

namespace ExpressoBits.Interactions
{
    [CreateAssetMenu(fileName = "RayCast Based Selector",menuName = "Expresso Bits/Interactions/Raycast Based Selector")]
    public class RayCastBasedInteractionsSelector : Selector
    {
        [Range(0.1f,10f)]
        [SerializeField] private float maxDistanceToSelect = 1.5f;

        public override void Check(Ray ray)
        {
            selection = null;
            if (Physics.Raycast(ray, out var hit, maxDistanceToSelect))
            {
                if(hit.transform.TryGetComponent(out IInteractable interactable))
                {
                    this.selection = interactable;
                }
            }
        }
    }
}