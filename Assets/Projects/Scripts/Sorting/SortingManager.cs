using System.Collections.Generic;
using Projects.Scripts.InteractiveObjects;
using UnityEngine;
using UnityEngine.Events;


namespace Projects.Scripts.Sorting
{
    /// <summary>
    /// 選別画面の管理。
    /// Washedラックの皿を種類ごとに分けるフローを制御する。
    /// </summary>
    public class SortingManager : MonoBehaviour
    {
        [Header("Window")]
        [Tooltip("選別画面の親オブジェクト（表示/非表示切り替え用）")]
        [SerializeField] private GameObject sortingWindow;

        [Header("Grid")]
        [Tooltip("選別画面用のグリッド表示")]
        [SerializeField] private SortingGridView sortingGridView;

        [Header("Events")]
        [SerializeField] private UnityEvent onSortingCompleted;

        private readonly List<SortingDish> _activeDishes = new();
        private readonly List<GameObject> _spawnedObjects = new();
        private Rack _currentRack;

        /// <summary>
        /// 選別画面を開始する
        /// </summary>
        public void StartSorting(Rack rack)
        {
            if (rack == null || rack.PlacementData == null) return;

            _currentRack = rack;
            rack.SetState(RackState.Sorting);

            sortingWindow.SetActive(true);
            InteractionLock.IsLocked = true;
            SpawnDishes(rack.PlacementData);
        }

        /// <summary>
        /// ラック上の皿をD&D可能なオブジェクトとして生成する
        /// </summary>
        private void SpawnDishes(RackPlacementData data)
        {
            var factory = new SortingDishFactory(
                sortingGridView.transform,
                sortingGridView.Geometry,
                sortingGridView.CellLocalSize,
                sortingGridView.ActiveTargets,
                sortingGridView.TargetRadius
            );

            foreach (var dish in data.Dishes)
            {
                var spawnResult = factory.Create(dish, OnDishSorted);
                _activeDishes.Add(spawnResult.SortingDish);
                _spawnedObjects.Add(spawnResult.GameObject);
            }
        }

        private void OnDishSorted(SortingDish dish)
        {
            _activeDishes.Remove(dish);

            if (_activeDishes.Count == 0)
            {
                CompleteSorting();
            }
        }

        private void CompleteSorting()
        {
            if (_currentRack != null)
            {
                _currentRack.ClearPlacementData();
                _currentRack.SetState(RackState.Empty);
                _currentRack = null;
            }

            Cleanup();
            sortingWindow.SetActive(false);
            InteractionLock.IsLocked = false;
            onSortingCompleted?.Invoke();
        }

        private void Cleanup()
        {
            foreach (var obj in _spawnedObjects)
            {
                if (obj != null) Destroy(obj);
            }

            _spawnedObjects.Clear();
            _activeDishes.Clear();
        }
    }
}
