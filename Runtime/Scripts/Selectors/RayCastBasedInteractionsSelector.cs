using Unity.Netcode;
using UnityEngine;

namespace ExpressoBits.Interactions
{
    public class RayCastBasedInteractionsSelector : Selector
    {

        public Ray LastRay => lastRay;
        public RaycastHit LastHit => lastHit;
        public Transform CameraTransform => cameraTransform;

        [Range(0.1f, 10f)]
        [SerializeField] private float maxDistanceToSelect = 1.5f;
        [SerializeField] private float additionalDistanceToSelector = 0f;

        #if UNITY_EDITOR
        [SerializeField] private bool debugLine;
        #endif

        [SerializeField] private bool isCustomTransformRayOrigin;
        [SerializeField] private Transform cameraTransform;
        private float distance;

        private RaycastHit lastHit;
        private Ray lastRay;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="additionalDistance"></param>
        public void SetAdditionalDistanceForSelector(float additionalDistance)
        {
            this.additionalDistanceToSelector = additionalDistance;
        }

        private Ray GetRay()
        {
            if(isCustomTransformRayOrigin)
            {
                if(cameraTransform == null) cameraTransform = Camera.main.transform;
                distance = Vector3.Distance(cameraTransform.position,transform.position);
                return new Ray(cameraTransform.position + cameraTransform.forward * distance/2f, cameraTransform.forward);
            }
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        public override void Check()
        {   
            lastRay = GetRay();

            if (Physics.Raycast(lastRay, out var hit, maxDistanceToSelect + additionalDistanceToSelector))
            {
                selection = hit.transform;
                lastHit = hit;
                return;
            }
            selection = null;
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(debugLine)
            {
                Color lastColor = Gizmos.color;
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(lastRay);
                Gizmos.color = lastColor;
            }
        }
        #endif
    }
}