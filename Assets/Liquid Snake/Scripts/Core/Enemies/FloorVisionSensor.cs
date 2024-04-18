using LiquidSnake.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LiquidSnake.Enemies
{
    [ExecuteAlways]
    public class FloorVisionSensor : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Radio de visión del sensor.")]
        private float viewRadius;

        [SerializeField]
        [Tooltip("Ángulo de visión del sensor, en ángulos.")]
        [Range(0, 360)]
        private float viewAngle;

        [SerializeField]
        [Tooltip("Objeto a detectar por el sensor.")]
        private Collider targetCollider;
        [SerializeField]
        [Tooltip("Objetos que se consideran como obstáculos para el sensor (i.e. que obstruyen visibilidad).")]
        private LayerMask obstacleMask;

        [SerializeField]
        [Tooltip("Offset a aplicar a la hora de pintar el sensor. Esto puede tener sentido si se quiere por ejemplo" +
            " dibujar el área de visión como proyectada en el suelo pero la detección real sucede a una altura diferente.")]
        private float verticalDrawOffset = 0f;

        public float meshResolution;
        public int edgeResolveIterations;
        public float edgeDstThreshold;

        public MeshFilter viewMeshFilter;
        Mesh viewMesh;

        void Start()
        {
            viewMesh = new Mesh();
            viewMesh.name = "View Mesh";
            viewMeshFilter.mesh = viewMesh;
        }

        void LateUpdate()
        {
            DrawFieldOfView();
        }

        public bool IsTargetVisible()
        {
            Vector3 targetPosition = targetCollider.ClosestPoint(transform.position);
            Vector3 dirToTarget = targetPosition - transform.position;
            Debug.DrawRay(transform.position, dirToTarget, Color.yellow);
            Vector3 planeDirToTarget = dirToTarget;
            planeDirToTarget.y = 0f;

            // ¿fuera del rango de visión (distancia)?
            // OJO: estamos comprobando distancia plana para que la región de detección tenga forma
            // de "queso". Esto quiere decir que da igual la altura del objetivo siempre y cuando
            // se encuentre en el área vertical que coincide con el sensor.
            float dstToTarget = planeDirToTarget.magnitude;
            if (dstToTarget > viewRadius) return false;

            // ¿fuera del rango de visión (ángulo)?
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                if (!Physics.Raycast(transform.position, dirToTarget.normalized, dstToTarget, obstacleMask, QueryTriggerInteraction.Ignore))
                {
                    return true;
                }
            }
            return false;
        } // IsTargetVisible

        void DrawFieldOfView()
        {
            int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
            float stepAngleSize = viewAngle / stepCount;
            List<Vector3> viewPoints = new List<Vector3>();
            ViewCastInfo oldViewCast = new ViewCastInfo();
            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
                ViewCastInfo newViewCast = ViewCast(angle);

                if (i > 0)
                {
                    bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                    if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                    {
                        EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                        if (edge.pointA != Vector3.zero)
                        {
                            viewPoints.Add(edge.pointA);
                        }
                        if (edge.pointB != Vector3.zero)
                        {
                            viewPoints.Add(edge.pointB);
                        }
                    }

                }

                viewPoints.Add(newViewCast.point);
                oldViewCast = newViewCast;
            }

            int vertexCount = viewPoints.Count + 1;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector3.up * verticalDrawOffset;
            for (int i = 0; i < vertexCount - 1; i++)
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
                vertices[i + 1].y += verticalDrawOffset;

                if (i < vertexCount - 2)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }

            viewMesh.Clear();

            viewMesh.vertices = vertices;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals();
        } // DrawFieldOfView


        EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
        {
            float minAngle = minViewCast.angle;
            float maxAngle = maxViewCast.angle;
            Vector3 minPoint = Vector3.zero;
            Vector3 maxPoint = Vector3.zero;

            for (int i = 0; i < edgeResolveIterations; i++)
            {
                float angle = (minAngle + maxAngle) / 2;
                ViewCastInfo newViewCast = ViewCast(angle);

                bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
                {
                    minAngle = angle;
                    minPoint = newViewCast.point;
                }
                else
                {
                    maxAngle = angle;
                    maxPoint = newViewCast.point;
                }
            }

            return new EdgeInfo(minPoint, maxPoint);
        }


        ViewCastInfo ViewCast(float globalAngle)
        {
            Vector3 dir = DirFromAngle(globalAngle, true);
            RaycastHit hit;

            if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask, QueryTriggerInteraction.Ignore))
            {
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
            }
            else
            {
                return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
            }
        }

        public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        public struct ViewCastInfo
        {
            public bool hit;
            public Vector3 point;
            public float dst;
            public float angle;

            public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
            {
                hit = _hit;
                point = _point;
                dst = _dst;
                angle = _angle;
            }
        }

        public struct EdgeInfo
        {
            public Vector3 pointA;
            public Vector3 pointB;

            public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
            {
                pointA = _pointA;
                pointB = _pointB;
            }
        }

    } // FloorVisionSensor

} // namespace LiquidSnake.Enemies