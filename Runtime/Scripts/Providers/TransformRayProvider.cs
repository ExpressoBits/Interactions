using UnityEngine;

namespace ExpressoBits.Interactions
{
    public class TransformRayProvider : MonoBehaviour, IRayProvider
    {

        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float distance;

        private void Awake()
        {
            cameraTransform = Camera.main.transform;
        }

        public Ray CreateRay()
        {
            distance = Vector3.Distance(cameraTransform.position,transform.position);
            return new Ray(cameraTransform.position + cameraTransform.forward * distance/2f, cameraTransform.forward);
        }
    }
}