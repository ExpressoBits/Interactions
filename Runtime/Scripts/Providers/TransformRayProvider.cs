using UnityEngine;

namespace ExpressoBits.Interactions
{
    public class TransformRayProvider : MonoBehaviour, IRayProvider
    {

        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float distance;

        public Ray CreateRay()
        {
            if(cameraTransform == null) cameraTransform = Camera.main.transform;
            distance = Vector3.Distance(cameraTransform.position,transform.position);
            return new Ray(cameraTransform.position + cameraTransform.forward * distance/2f, cameraTransform.forward);
        }
    }
}