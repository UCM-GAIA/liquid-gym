using LiquidSnake.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LiquidSnake.Interactions
{
    /// <summary>
    /// Gestiona las interacciones de un Game Object con los objetos 
    /// interactuables ("Interactable"). Cuando entra en el rango 
    /// para interactuar con un objeto se le indica que puede interactuar 
    /// y guarda dicho objeto (revisar, quizá no sea la mejor opción)
    /// </summary>
    public class InteractionManager : MonoBehaviour, IResetteable
    {
        private List<Interactable> interactableObjects;


        private void Start()
        {
            Reset();
        }

        /// <summary>
        /// Trigger interaction between this component and the closest interactable object in its list of 
        /// enabled interactions.
        /// </summary>
        public void TriggerInteraction()
        {
            if (interactableObjects.Count() > 0)
            {
                interactableObjects.Sort(delegate (Interactable x, Interactable y)
                {
                    return x.distanceToCharacter.CompareTo(y.distanceToCharacter);
                });

                Interactable obj = interactableObjects.First();
                obj.Interacted(gameObject);
            }
        }

        /// <summary>
        /// Notify interactor of a new interaction being available in its surroundings.
        /// </summary>
        /// <param name="interactableObject">Object that can now be interacted with.</param>
        public void EnableInteraction(Interactable interactableObject)
        {
            if (!interactableObjects.Contains(interactableObject))
            {
                interactableObjects.Add(interactableObject);
                interactableObject.OnInteractableDisabled += DisableInteraction;
            }
        }

        /// <summary>
        /// Notify interactor of an interaction no longer being available in its surroundings.
        /// </summary>
        public void DisableInteraction(Interactable interactableObject)
        {
            if (interactableObjects.Contains(interactableObject))
            {
                interactableObjects.Remove(interactableObject);
                interactableObject.OnInteractableDisabled -= DisableInteraction;
            }
        }

        public void Reset()
        {
            interactableObjects = new List<Interactable>();
        } // Reset
    }
}
