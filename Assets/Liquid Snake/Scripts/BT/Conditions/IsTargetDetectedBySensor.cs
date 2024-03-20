using BBUnity.Conditions;
using LiquidSnake.Enemies;
using Pada1.BBCore;
using UnityEngine;


namespace LiquidSnake.BT.Conditions
{
    /// <summary>
    /// It is a condition of perception to check if the objective is in view depending on a given range.
    /// </summary>
    [Condition("LiquidSnake/IsTargetDetectedBySensor")]
    [Help("Checks whether there is a visible target within the sensor component.")]
    public class IsTargetDetectedBySensor : GOCondition
    {
        [InParam("visionSensorObject")]
        [Help("Vision Sensor used by this condition with a VisionSensor Component attached")]
        public GameObject visionSensorObject;

        [OutParam("closestDetectedTarget")]
        [Help("Closest detected game object within the sensor area")]
        public GameObject closestDetectedTarget;

        /// <summary>
        /// Comprueba si el sensor ha detectado alg�n objeto en su regi�n de detecci�n
        /// y, en tal caso, establece el m�s cercano dentro de la regi�n en closestDetectedTarget 
        /// en la variable closestDetectedTarget.
        /// </summary>
        public override bool Check()
        {
            if (visionSensorObject != null && visionSensorObject.TryGetComponent<VisionSensor>(out var visionSensor))
            {
                closestDetectedTarget = visionSensor.DetectClosestTarget();
                return closestDetectedTarget != null;
            }
            return false;
        }
    } // IsTargetDetectedBySensor
} // namespace LiquidSnake.BT.Conditions