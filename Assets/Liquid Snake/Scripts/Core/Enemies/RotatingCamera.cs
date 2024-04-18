using LiquidSnake.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LiquidSnake.Enemies
{
    public class RotatingCamera : MonoBehaviour, IResetteable
    {
        [SerializeField]
        [Tooltip("Degrees per second that the camera will rotate.")]
        private float rotationSpeed = 30f; // Degrees per second
        [SerializeField]
        [Tooltip("Initial Rotation of the camera, in degrees around the Y axis.")]
        [Range(0f, 359f)]
        private float rotationA = 0f; // Initial rotation
        [SerializeField]
        [Tooltip("Target rotation of the camera, in degrees around the Y axis.")]
        [Range(0f, 359f)]
        private float rotationB = 90f; // Target rotation
        [SerializeField]
        [Tooltip("Seconds to wait between rotations. That is, after reaching rotationA from rotationB and viceversa.")]
        private float waitingSeconds = 2f; // Seconds to wait between rotations
        [SerializeField]
        [Tooltip("Percentual awareness increase per second of the target being detected within the camera sensor.")]
        private float awarenessIncreaseRate = 10f; // Percentage per second
        [SerializeField]
        [Tooltip("Percentual awareness decrease per second of the target NOT being within the camera sensor.")]
        private float awarenessDecreaseRate = 5f; // Percentage per second

        [Header("Ajustes de Percepción")]
        [SerializeField]
        [Tooltip("Sensor empleado para percibir a los objetivos de esta cámara.")]
        private FloorVisionSensor sensor;

        [SerializeField]
        [Tooltip("Imagen de barra de progreso de awareness.")]
        private Image awarenessImage;
        [SerializeField]
        [Tooltip("Texto incluido dentro de la UI de awareness (?/!).")]
        private TextMeshProUGUI awarenessText;
        [SerializeField]
        [Tooltip("Canvas con el estado de awareness de la cámara.")]
        private GameObject awarenessCanvas;

        [SerializeField]
        [Tooltip("Evento al que suscribirse para recibir la notificación de que el personaje ha sido detectado completamente.")]
        private UnityEvent CharacterCaught;

        public Transform gameCamera;

        private bool inAlertMode = false;
        private float awareness = 0f;
        private bool rotating = false;
        private bool waiting = false;
        private float remainingWaitTime = 0f;
        private Color originalUIColor;
        /// <summary>
        /// Determina si la cámara está actualmente rotando hacia
        /// el punto B (true) o hacia el punto A (false).
        /// </summary>
        private bool rotateTowardsB = true;

        private void Start()
        {
            originalUIColor = awarenessImage.color;
            Reset();
        } // Start

        private void OnValidate()
        {
            transform.rotation = Quaternion.Euler(0f, rotationA, 0f);
        }

        private void Update()
        {
            // la cámara está en alerta si el sensor ha detectado a su objetivo
            // o si no hay objetivo, pero lo hubo hace poco y la alerta aún sigue viva.
            bool targetFound = sensor.IsTargetVisible();
            inAlertMode = targetFound || awareness > 0;

            // si no estamos en modo alerta, estamos o en pausa entre rotaciones o rotando
            if (!inAlertMode)
            {
                // asumiendo que no estemos rotando pero tampoco esperando, es momento de iniciar
                // la rotación hacia el siguiente punto objetivo.
                if (!rotating && !waiting)
                {
                    StartRotation();
                }
                // si sí que estamos rotando, simplemente aplicamos un tick de la rotación
                // en base al delta time.
                else if (rotating)
                {
                    RotateCamera();
                }
                // en el caso de espera, nos limitamos a hacer tick al tiempo restante.
                else if (waiting)
                {
                    Wait(Time.deltaTime);
                }
            }
            else
            {
                // si estamos en alerta, actualizamos la fracción de awareness según si el target ha sido encontrado o no.
                UpdateAwarenessUI(targetFound);
            }
        } // Update

        private void StartRotation()
        {
            rotateTowardsB = !rotateTowardsB;
            rotating = true;
        } // StartRotation

        private void RotateCamera()
        {
            float targetRotation = rotateTowardsB ? rotationB : rotationA;
            float currentRotation = transform.eulerAngles.y;
            if (Mathf.Abs(currentRotation - targetRotation) < 0.1f)
            {
                // objetivo alcanzado, pasamos a la fase de espera
                transform.rotation = Quaternion.Euler(0f, targetRotation, 0f);
                rotating = false;
                waiting = true;
                remainingWaitTime = waitingSeconds;
            }
            else
            {
                // paso hacia la rotación objetivo
                float step = rotationSpeed * Time.deltaTime;
                float nextRotation = Mathf.MoveTowardsAngle(currentRotation, targetRotation, step);
                transform.rotation = Quaternion.Euler(0f, nextRotation, 0f);
            }
        } // RotateCamera

        private void Wait(float waitedTime)
        {
            remainingWaitTime = Mathf.Max(0f, remainingWaitTime - waitedTime);
            if (remainingWaitTime <= 0f) waiting = false;
        } // Wait


        public void Reset()
        {
            transform.rotation = Quaternion.Euler(0f, rotationA, 0f);
            awareness = 0f;
            UpdateAwarenessUI();
            StartRotation();
        } // Reset

        private void UpdateAwarenessUI(bool increaseAwareness = false)
        {
            if (increaseAwareness) awareness = Mathf.Min(1f, awareness + awarenessIncreaseRate * Time.deltaTime);
            else awareness = Mathf.Max(0f, awareness - awarenessDecreaseRate * Time.deltaTime);

            awarenessCanvas.SetActive(awareness > 0f);
            if (awareness >= 1f)
            {
                awarenessImage.color = Color.red;
                awarenessText.text = "!";
                CharacterCaught?.Invoke();
            }
            else if (awareness > 0f)
            {
                awarenessImage.color = originalUIColor;
                awarenessText.text = "?";
            }

            awarenessImage.fillAmount = awareness;
        } // UpdateAwarenessUI

    } // RotatingCamera

} // namespace LiquidSnake.Enemies
