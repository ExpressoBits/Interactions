using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExpressoBits.Interactions
{
    public abstract class ActionInteractor : MonoBehaviour
    {
        [SerializeField] protected List<ActionType> actionTypes;
        protected Interactor interactor;

        public Interactor Interactor => interactor;
        public List<ActionType> ActionTypes => actionTypes;

        internal void SetInteractor(Interactor interactor)
        {
            this.interactor = interactor;
        }

        public abstract void Interact(ActionType actionType, Interactable interactable);

        public abstract bool TryTriggerAction(Interactable interactable, out ActionType actionType);

        public abstract string GetKeyMessage(Interactable interactable, ActionType actionType);
    }
}

