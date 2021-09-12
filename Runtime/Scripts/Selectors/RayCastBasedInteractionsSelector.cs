using UnityEngine;

namespace ExpressoBits.Interactions
{
    public class RayCastBasedInteractionsSelector : MonoBehaviour, ISelector
    {

        public Transform Selection => selection;
        
        private Transform selection;
        [Range(0.1f,10f)]
        [SerializeField] private float maxDistanceToSelect = 1.5f;

        public void Check(Ray ray)
        {
            selection = null;
            if (Physics.Raycast(ray, out var hit, maxDistanceToSelect))
            {
                var selection = hit.transform;
                if(selection.TryGetComponent(out IInteractable interactable))
                {
                    this.selection = selection;
                }
            }
        }

    }
}