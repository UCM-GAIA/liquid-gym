using LiquidSnake.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace LiquidSnake.LevelObjects
{
    public class CollectibleObjectTracker : MonoBehaviour, IResetteable
    {

        [SerializeField]
        private List<CollectibleObject> objectsToCollect = new List<CollectibleObject>();

        [SerializeField]
        private TextMeshProUGUI textMeshProUGUI;

        [SerializeField]
        private UnityEvent OnAllObjectsCollected;


        private int collectedObjectsCount = 0;

        private void OnEnable()
        {
            foreach (var item in objectsToCollect)
            {
                item.OnCollected += HandleObjectCollected;
            }
        }

        private void OnDisable()
        {
            foreach (var item in objectsToCollect)
            {
                item.OnCollected -= HandleObjectCollected;
            }
        }

        private void HandleObjectCollected()
        {
            collectedObjectsCount++;
            UpdateCollectionUI();

            if (collectedObjectsCount >= objectsToCollect.Count) OnAllObjectsCollected?.Invoke();
        } // HandleObjectCollected

        private void UpdateCollectionUI()
        {
            textMeshProUGUI.text = collectedObjectsCount >= objectsToCollect.Count ? "EXIT!" : $"{collectedObjectsCount}/{objectsToCollect.Count} <sprite=12>";
        } // UpdateCollectionUI

        public void Reset()
        {
            collectedObjectsCount = 0;
            UpdateCollectionUI();
        } // Reset

    } // CollectibleObjectTracker

} // namespace LiquidSnake.LevelObjects
