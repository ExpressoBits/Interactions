using UnityEngine;

namespace ExpressoBits.Interactions
{
    public class MouseScreenRayProvider : MonoBehaviour, IRayProvider
    {

        public Ray CreateRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);;
        }
    }
}