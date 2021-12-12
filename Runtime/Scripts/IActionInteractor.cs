namespace ExpressoBits.Interactions
{
    public interface IActionInteractor
    {
        public ActionType[] ActionTypes { get; }

        public void Interact(ActionType actionType, Interactable interactable);
    }
}

