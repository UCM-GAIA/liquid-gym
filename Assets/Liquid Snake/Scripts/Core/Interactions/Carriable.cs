using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiquidSnake.Interactions
{
    /// <summary>
    /// Entidad interactuable la cual se transporta y es lanzada en la dirección
    /// en la que está mirando el personaje que la lleva.
    /// </summary>
    public class Carriable : Interactable
    {
        private bool carried = false;

        [SerializeField]
        private GameObject playerHand;
        private float dist;

        /// <summary>
        /// Cuando es interaccionado su comportamiento depende de si el player
        /// lo está transportando o no.
        /// </summary>
        /// <param name="interactor"></param>
        public override void Interacted(GameObject interactor)
        {
            if (carried)
            {
                carried = false;
                //transform.position = new Vector3(0,0,0);
                transform.parent = null;
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<Rigidbody>().isKinematic = false;
                Debug.Log(interactor.transform.forward);
                GetComponent<Rigidbody>().AddForce((interactor.transform.forward + new Vector3(0, 0.5f, 0)) * 700);
                distanceToCharacter = dist;
            }
            else
            {
                carried = true;
                transform.position = playerHand.transform.position;
                transform.parent = playerHand.transform;
                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<Rigidbody>().isKinematic = true;
                dist = distanceToCharacter; // Esto ya no haría falta guardarlo porque se lanza, era simplemente para la primera iteración que volvía a dejar el objeto en su sitio.
                distanceToCharacter = 0;

            }
        }
    }

}
