using UnityEngine;

namespace LiquidSnake.LevelObjects
{
    public class RotateAndScale : MonoBehaviour
    {
        [SerializeField]
        private Vector3 scaleA;
        [SerializeField]
        private Vector3 scaleB;
        [SerializeField]
        [Tooltip("Time needed to move scale from A to B.")]
        private float scalingSpeed = 1f;
        [SerializeField]
        [Tooltip("Angles to rotate per second around Y axis.")]
        private float rotationSpeed;
        [SerializeField]

        private void Update()
        {
            // Rotate the object continuously around the Y axis
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

            // Fluctuate between scaleA and scaleB
            transform.localScale = Vector3.Lerp(scaleA, scaleB, Mathf.PingPong(Time.time * scalingSpeed, 1));
        } // Update

    } // RotateAndScale

} // namespace LiquidSnake.LevelObjects

