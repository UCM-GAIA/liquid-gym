using LiquidSnake.Utils;
using UnityEngine;

namespace LiquidSnake.LevelObjects
{
    public class CollectibleObject : MonoBehaviour, IResetteable
    {

        public delegate void CollectibleAction();
        [SerializeField]
        public event CollectibleAction OnCollected;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Trigger the event
                OnCollected?.Invoke();

                // Disable the object
                gameObject.SetActive(false);
            }
        } // OnTriggerEnter

        public void Reset()
        {
            gameObject.SetActive(true);
        } // Reset

    } // CollectibleObject

} // namespace LiquidSnake.LevelObjects
