using UnityEngine;

namespace LiquidSnake.Interactions
{
    public interface ICollectItemInteractor
    {
        /// <summary>
        /// Performs a collection action over given item.
        /// </summary>
        /// <param name="item">Item to be collected by the interactor.</param>
        void CollectItem(CollectableItem item);
    }

    [System.Serializable]
    public class CollectableItem
    {
        public string name;
    }

    /// <summary>
    /// Entidad interactuable de recompensa que el jugador puede recoger
    /// </summary>
    public class Collectable : Interactable
    {
        private bool collected = false;
        [SerializeField] private CollectableItem collectableInfo;
        /// <summary>
        /// Cuando se interactúa con el objeto, se añade a la lista de
        /// coleccionables del jugador
        /// </summary>
        /// <param name="interactor"></param>
        public override void Interacted(GameObject interactor)
        {
            ICollectItemInteractor collectInteractor = interactor.GetComponent<ICollectItemInteractor>();
            if (!collected && collectInteractor != null)
            {
                collectInteractor.CollectItem(collectableInfo);
                collected = true;
                gameObject.SetActive(false);

                if (interactor.TryGetComponent<InteractionManager>(out var characterInteraction))
                {
                    characterInteraction.DisableInteraction(this);
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            collected = false;
        }
    }

}
