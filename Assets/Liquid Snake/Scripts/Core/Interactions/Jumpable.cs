using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiquidSnake.Interactions
{
    public interface IJumpInteractor
    {
        /// <summary>
        /// Performs a jump action to given position.
        /// </summary>
        /// <param name="position">Position interactor should jump to.</param>
        void JumpToPosition(Vector3 position);
    }

    /// <summary>
    /// Entidad que al interactuar con ella el personaje pasa por encima.
    /// Necesita tener un objeto vacío que utiliza para establecer el eje
    /// sobre el que se hace la transformación de la posición del personaje.
    /// </summary>
    public class Jumpable : Interactable
    {
        [SerializeField]
        private GameObject axisPoint;

        /// <summary>
        /// base seconds to wait before jumpable object can be interacted with again
        /// </summary>
        private float cooldownBase = 0.5f;

        /// <summary>
        /// seconds to wait before being able to interact with this object
        /// </summary>
        private float activeCooldown = 0f;

        private void Update()
        {
            if(activeCooldown > 0f)
            {
                activeCooldown -= Time.deltaTime;
                activeCooldown = Mathf.Max(0f, activeCooldown);
            }
        }

        private void Start()
        {
            if (axisPoint == null)
                axisPoint = transform.GetChild(0).gameObject;
        } // Start

        public override void Interacted(GameObject interactor)
        {
            if (activeCooldown > 0f) return;

            IJumpInteractor jumpInteractor = interactor.GetComponent<IJumpInteractor>();
            if (jumpInteractor != null)
            {
                Vector2 newPos = PointMirrored2D(interactor.transform.position, axisPoint.transform.position, transform.position);
                jumpInteractor.JumpToPosition(new Vector3(newPos.x, interactor.transform.position.y, newPos.y));
                activeCooldown = cooldownBase;
            }
        } // Interacted

        private Vector2 PointMirrored2D(Vector3 point, Vector3 jumpAxisNodeA, Vector3 jumpAxisNodeB)
        {
            // Puntos en 2D que configuran el eje de salto
            Vector2 P1 = new Vector2(jumpAxisNodeA.x, jumpAxisNodeA.z);
            Vector2 P2 = new Vector2(jumpAxisNodeB.x, jumpAxisNodeB.z);

            // Punto a reflejar en 2D
            Vector2 P = new Vector2(point.x, point.z);

            // Form a unit vector in the direction of the line.
            Vector2 lineDirection = (P2 - P1).normalized;

            // Rotate the vector 90 degrees in the XY plane
            // to get a vector perpendicular to the mirror line.
            Vector2 perpendicular = new Vector2(-lineDirection.y, lineDirection.x);

            // Take away a's perpendicular offset from this line, twice.
            // Once to flatten a onto the line, and a second time to make b,
            // an equal distance away on the opposite side of the line.
            Vector2 res = P - 2 * Vector3.Dot((P - P1), perpendicular) * perpendicular;

            return res;
        } // PointMirrored2D

    } // Jumpable
}
