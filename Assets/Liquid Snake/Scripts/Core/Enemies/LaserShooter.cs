using LiquidSnake.Utils;
using UnityEngine;


namespace LiquidSnake.Enemies
{
    /// <summary>
    /// Componente dedicado a gestionar la acci�n de disparo del enemigo.
    /// </summary>
    public class LaserShooter : MonoBehaviour, IResetteable
    {

        //--------------------------------------------------------------------------------
        //                        Propiedades del inspector
        //--------------------------------------------------------------------------------
        #region Propiedades del inspector
        /// <summary>
        /// Punto desde el que se disparar� el l�ser del enemigo.
        /// </summary>
        [SerializeField]
        private Transform shootPoint;

        /// <summary>
        /// Prefab a instanciar como l�ser utilizado para el disparo.
        /// </summary>
        [SerializeField]
        private GameObject bullet;

        /// <summary>
        /// Velocidad con la que se lanzar� el proyectil.
        /// </summary>
        [SerializeField]
        private float velocity = 30f;

        /// <summary>
        /// Tiempo en segundos que debe transcurrir para poder volver a disparar.
        /// </summary>
        [SerializeField]
        private float cooldown = 0.3f;
        #endregion

        //--------------------------------------------------------------------------------
        //                        Propiedades privadas
        //--------------------------------------------------------------------------------
        #region Propiedades privadas
        /// <summary>
        /// Tiempo a esperar hasta el pr�ximo disparo.
        /// </summary>
        private float _timeUntilNextShoot = 0f;
        #endregion

        //--------------------------------------------------------------------------------
        //                        Ciclo de vida del componente
        //--------------------------------------------------------------------------------
        #region Ciclo de vida del componente
        public void Start()
        {
            // si no se ha especificado un punto de disparo expl�cito, buscamos por nombre en la jerarqu�a del objeto.
            if (shootPoint == null)
            {
                shootPoint = gameObject.transform.Find("shootPoint");
                if (shootPoint == null)
                {
                    Debug.LogWarning("Shoot point not specified. Laser Shooter will not work " +
                                     "for " + gameObject.name);
                }
            }
        } // Start

        private void Update()
        {
            _timeUntilNextShoot -= Time.deltaTime;
            _timeUntilNextShoot = Mathf.Max(0f, _timeUntilNextShoot);
        } // Update
        #endregion


        //--------------------------------------------------------------------------------
        //                        API p�blica del componente
        //--------------------------------------------------------------------------------

        #region API p�blica del componente
        /// <summary>
        /// Dispara el proyectil especificado desde el punto shootPoint.
        /// Si se indica el par�metro opcional objetivo, el disparo se har� en direcci�n a dicho objeto.
        /// En caso contrario, el disparo se realiza en la direcci�n del vector forward del enemigo.
        /// </summary>
        /// <returns>true si el disparo pudo realizarse con �xito</returns>
        public bool Shoot(Transform target = null)
        {
            if (shootPoint == null || _timeUntilNextShoot > 0f)
            {
                return false;
            }

            // Instantiate the bullet prefab.
            GameObject newBullet = Instantiate(
                                        bullet, shootPoint.position,
                                        shootPoint.rotation * bullet.transform.rotation
                                    );
            // Give it a velocity
            if (newBullet.GetComponent<Rigidbody>() == null)
                // Safeguard test, altough the rigid body should be provided by the
                // prefab to set its weight.
                newBullet.AddComponent<Rigidbody>();

            newBullet.GetComponent<Rigidbody>().velocity = velocity * (target == null ? shootPoint.forward : (target.transform.position - transform.position).normalized);
            // reseteo del cooldown
            _timeUntilNextShoot = cooldown;
            return true;
        } // Shoot

        #endregion

        //--------------------------------------------------------------------------------
        //                        Implementaci�n de IResetteable
        //--------------------------------------------------------------------------------

        #region Implementaci�n de IResetteable
        public void Reset()
        {
            _timeUntilNextShoot = 0f;
        } // Reset
        #endregion
    }

} // namespace LiquidSnake.Enemies