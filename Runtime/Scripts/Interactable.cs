using Unity.Netcode;
using UnityEngine;

namespace ExpressoBits.Interactions
{
    /// <summary>
    /// An interface to elements that will be interactable by the interactor
    /// </summary>
    public class Interactable : NetworkBehaviour
    {
        
        public bool IsShowPreviewMessage => isShowPreviewMessage;
        public InteractableAction DefaultAction => defaultAction;

        [SerializeField] private InteractableAction defaultAction;
        [SerializeField] private InteractableAction[] actions;

        [SerializeField] private bool isShowPreviewMessage;

        public void DefaultInteract(Interactor interactor)
        {
            defaultAction.Action(interactor);
        }

        public string PreviewMessage()
        {
            return DefaultAction.PreviewMessage;
        }
    }
}

