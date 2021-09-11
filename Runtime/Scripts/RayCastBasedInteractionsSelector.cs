using UnityEngine;

namespace ExpressoBits.Interactions
{
    public class RayCastBasedInteractionsSelector : MonoBehaviour, ISelector
    {
        private Transform selection;

        public float maxDistanceToSelect = 1.5f;

        public void Check(Ray ray)
        {
            selection = null;
            if (Physics.Raycast(ray, out var hit, maxDistanceToSelect))
            {
                var selection = hit.transform;
                IInteractable interaction = selection.GetComponent<IInteractable>();
                if (interaction != null)
                {
                    this.selection = selection;
                }
            }
        }

        public Transform GetSelection()
        {
            return selection;
        }
    }
}