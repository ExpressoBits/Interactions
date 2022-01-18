using UnityEngine;

namespace ExpressoBits.Interactions
{
    [CreateAssetMenu(fileName = "Interactable Action", menuName = "Expresso Bits/Interactions/Interactable Action")]
    public class ActionType : ScriptableObject
    {
        public string DefaultPreviewMessage => defaultPreviewMessage;
        [SerializeField] private Sprite icon;
        [SerializeField] private string defaultPreviewMessage = "for Interact";
    }
}

