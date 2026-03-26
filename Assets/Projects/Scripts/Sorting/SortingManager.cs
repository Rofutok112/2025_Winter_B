using System;
using System.Collections.Generic;
using Projects.Scripts.Control;
using Projects.Scripts.InteractiveObjects;
using UnityEngine;


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

        [Header("Targets")]
        [SerializeField] private SortingTargetGroup sortingTargetGroup;

        [Header("Input")]
        [SerializeField] private InputStateRouter inputStateRouter;

        private readonly List<SortingDish> _activeDishes = new();
        private readonly List<GameObject> _spawnedObjects = new();
        private Rack _currentRack;
        private bool _isSortingTransitioning;
        public event Action<Rack> SortingStarted;
        public event Action<int> SortingScoreConfirmed;
        public event Action SortingCompleted;

        private void Awake()
        {
            ValidateReferences();
        }

        /// <summary>
        /// 選別画面を開始する
        /// </summary>
        public void StartSorting(Rack rack)
        {
            if (rack == null || rack.PlacementData == null || _currentRack != null || _isSortingTransitioning || !ValidateReferences()) return;

            _isSortingTransitioning = true;
            _currentRack = rack;
            rack.SetState(RackState.Sorting);

            sortingWindow.SetActive(true);
            ResetTargets();
            SpawnDishes(rack.PlacementData);
            SortingStarted?.Invoke(rack);
            sortingGridView.PlayOpeningAnimation(() =>
            {
                inputStateRouter?.SetOperationState(InputOperationState.Sorting);
                _isSortingTransitioning = false;
            });
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
                sortingTargetGroup != null ? sortingTargetGroup.ActiveTargets : null,
                sortingTargetGroup != null ? sortingTargetGroup.TargetRadius : 1.5f,
                InputTargetRole.Sorting
            );

            foreach (var dish in data.Dishes)
            {
                var spawnResult = factory.Create(dish, OnDishSorted);
                _activeDishes.Add(spawnResult.SortingDish);
                _spawnedObjects.Add(spawnResult.GameObject);
            }
        }

        private void OnDishSorted(SortingDish dish, int scorePoints)
        {
            SortingScoreConfirmed?.Invoke(Mathf.Max(0, scorePoints));
            _activeDishes.Remove(dish);

            if (_activeDishes.Count == 0)
            {
                CompleteSorting();
            }
        }

        private void CompleteSorting()
        {
            if (_isSortingTransitioning) return;

            _isSortingTransitioning = true;

            if (_currentRack != null)
            {
                _currentRack.ClearPlacementData();
                _currentRack.SetState(RackState.Empty);
                _currentRack = null;
            }

            inputStateRouter?.ResetToDefault();
            sortingGridView.PlayClosingAnimation(() =>
            {
                Cleanup();
                sortingWindow.SetActive(false);
                _isSortingTransitioning = false;
                SortingCompleted?.Invoke();
            });
        }

        private void Cleanup()
        {
            foreach (var obj in _spawnedObjects)
            {
                if (obj != null) Destroy(obj);
            }

            _spawnedObjects.Clear();
            _activeDishes.Clear();
            ResetTargets();
        }

        public void ForceCloseSorting()
        {
            _isSortingTransitioning = false;
            inputStateRouter?.ResetToDefault();

            if (_currentRack != null)
            {
                _currentRack.SetState(RackState.Washed);
                _currentRack = null;
            }

            Cleanup();

            if (sortingWindow != null)
            {
                sortingWindow.SetActive(false);
            }
        }

        private void ResetTargets()
        {
            if (sortingTargetGroup == null) return;

            foreach (var target in sortingTargetGroup.ActiveTargets)
            {
                target?.ResetStack();
            }
        }

        private bool ValidateReferences()
        {
            if (sortingWindow == null ||
                sortingGridView == null ||
                sortingTargetGroup == null ||
                inputStateRouter == null)
            {
                Debug.LogWarning($"{nameof(SortingManager)} is missing scene references.", this);
                return false;
            }

            return true;
        }
    }
}
