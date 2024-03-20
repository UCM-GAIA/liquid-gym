using LiquidSnake.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace LiquidSnake.Enemies
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class VisionSensor : MonoBehaviour, IResetteable
    {


        //----------------------------------------------------------------------------
        //                      Atributos de Inspector
        //----------------------------------------------------------------------------
        #region Atributos de Inspector
        [SerializeField]
        [Tooltip("Range of detection, in degrees.")]
        private float detectionAngles = 90f;

        [SerializeField]
        [Tooltip("Height of the detection area.")]
        private float sensorHeight = 1f;

        [SerializeField]
        [Tooltip("Depth of the detection area.")]
        private float sensorDepth = 5f;

        [SerializeField]
        [Tooltip("Vertical offset of the detection area.")]
        private float verticalOffset = 0f;

        [SerializeField]
        private string[] detectableTags;

        [SerializeField]
        private Material targetFoundMaterial;
        #endregion


        //----------------------------------------------------------------------------
        //                      Atributos Privados
        //----------------------------------------------------------------------------
        private Mesh _mesh;
        private MeshRenderer _meshRenderer;
        private List<GameObject> _objectsInArea = new List<GameObject>();
        private Material _defaultMaterial;

        //----------------------------------------------------------------------------
        //                      Ciclo de vida del componente
        //----------------------------------------------------------------------------

        private void OnValidate()
        {
            _defaultMaterial = GetComponent<MeshRenderer>().sharedMaterial;
            BuildMesh();
        }

        private void BuildMesh()
        {
            // Construcción del triángulo de visión (cosmético)
            _mesh = GetComponent<MeshFilter>().sharedMesh != null ? GetComponent<MeshFilter>().sharedMesh : new Mesh();
            _mesh.Clear();

            Vector3[] newVertices = new Vector3[5];
            // esquina central (O)
            newVertices[0] = (Vector3.up * verticalOffset);

            float detectionRadians = Mathf.Deg2Rad * detectionAngles;

            // esquinas Izquierda (A) y Derecha (B)
            float xB = Mathf.Tan(detectionRadians / 2) * sensorDepth;
            float xA = -xB;
            float zAB = sensorDepth;

            // A
            newVertices[1] = (new Vector3(xA, verticalOffset - sensorHeight / 2, zAB));
            newVertices[2] = (new Vector3(xA, verticalOffset + sensorHeight / 2, zAB));
            // B
            newVertices[3] = (new Vector3(xB, verticalOffset - sensorHeight / 2, zAB));
            newVertices[4] = (new Vector3(xB, verticalOffset + sensorHeight / 2, zAB));
            _mesh.vertices = newVertices;

            //mesh.uv = newUV;
            _mesh.triangles = new int[] {
                0, 1, 2,
                0, 2, 4,
                0, 4, 3,
                0, 1, 3,
                // 1, 2, 4,
                // 1, 4, 3
            };

        } // BuildMesh

        private void OnTriggerEnter(Collider other)
        {
            if (detectableTags.Any(other.CompareTag))
            {
                _objectsInArea.Add(other.gameObject);
            }
        } // OnTriggerEnter

        private void OnTriggerExit(Collider other)
        {
            _objectsInArea.Remove(other.gameObject);
        } // OnTriggerExit

        public void Reset()
        {
            _objectsInArea.Clear();
        } // Reset

        /// <summary>
        /// Devuelve el objeto detectado más cercano al punto de origen del sensor
        /// (esto implica que el objeto sea visible y no sólo que se encuentre dentro
        /// del área del mesh). Si el resultado de esta operación no es null, modifica
        /// el material del mesh renderer con el material de alerta para notificar de que
        /// el sensor está percibiendo al objetivo.
        /// </summary>
        public GameObject DetectClosestTarget()
        {
            float minDistance = Mathf.Infinity;
            GameObject closest = null;

            // punto de origen de la visión habiendo aplicado el offset vertical
            // (desde aquí realizaremos el raycast para buscar objetos).
            Vector3 sightOrigin = transform.position + Vector3.up * verticalOffset;

            foreach (var obj in _objectsInArea)
            {
                Vector3 targetPos = obj.transform.position + Vector3.up * 0.5f;
                Vector3 dir = (targetPos - sightOrigin);
                RaycastHit hit;
                // TODO: soporte para LayerMask
                if (Physics.Raycast(sightOrigin, dir.normalized, out hit))
                {
                    // No hay nada que obstruya la visión desde nuestro punto hasta el objeto,
                    // y además la distancia al objeto en cuestión es menor que la mínima registrada.
                    if (hit.collider.gameObject == obj)
                    {
                        float d = dir.sqrMagnitude;
                        if (d < minDistance)
                        {
                            minDistance = d; closest = obj;
                        }
                    }
                }
            }

            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.material = closest != null ? targetFoundMaterial : _defaultMaterial;
            return closest;
        } // GetClosestDetectedTarget

    } // VisionSensor

} // namespace LiquidSnake.Enemies