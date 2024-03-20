using LiquidSnake.Utils;
using UnityEngine;

namespace LiquidSnake.Interactions
{

    public delegate void InteractableDisabledEvent(Interactable interactable);

    /// <summary>
    /// Lógica base de los objetos con los que se puede interactuar. 
    /// Avisan al player si está dentro del rango de interacción con ellos y
    /// se añaden a la lista de objetos con los que éste puede interaccionar.
    /// </summary>
    public class Interactable : MonoBehaviour, IResetteable
    {
        public float distanceToCharacter;

        public event InteractableDisabledEvent OnInteractableDisabled;

        /// <summary>
        /// When the collider is triggered, the collider its required to have an interactionmanager. If not its ignore.
        /// If the collider gameobject has an interaction manager, we enable the interaction in that manager.
        /// </summary>
        /// <param name="col"></param>
        private void OnTriggerEnter(Collider col)
        {
            // Interaction manager attached to colliding object
            if (CanItBeInteracted(col))
            {

                InteractionManager objectInteractionManager = col.GetComponent<InteractionManager>();
                if (objectInteractionManager != null)
                {
                    objectInteractionManager.EnableInteraction(this);
                    distanceToCharacter = Vector3.Distance(col.gameObject.transform.position, transform.position);
                }
            }
        }

        /// <summary>
        /// When the collider disables the trigger the collider its required to have an interactionmanager. If not its ignore.
        /// If the collider gameobject has an interaction manager, we disable the interaction in that manager.
        /// </summary>
        /// <param name="col"></param>
        private void OnTriggerExit(Collider col)
        {
            if (CanItBeInteracted(col))
            {
                // Interaction manager attached to colliding object
                InteractionManager objectInteractionManager = col.GetComponent<InteractionManager>();
                if (objectInteractionManager != null)
                {
                    objectInteractionManager.DisableInteraction(this);
                }
            }
        }

        private void OnDisable()
        {
            OnInteractableDisabled?.Invoke(this);
        }

        virtual public void Reset()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Method that defines the interact action with the game object by the interactor
        /// </summary>
        /// <param name="interactor">gameobject that is trying to interact with the object that inherits this interface</param>
        virtual public void Interacted(GameObject interactor) { }

        /// <summary>
        /// The method checks if the component that its colliding can be interacted or its going to be ignore
        /// </summary>
        /// <param name="collider"></param>
        /// <returns>true by default</returns>
        virtual public bool CanItBeInteracted(Collider collider) { return true; }
    }
}
