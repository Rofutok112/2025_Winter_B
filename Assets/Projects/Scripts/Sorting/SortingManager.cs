using System;
using System.Collections.Generic;
using System.Linq;
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
        [Header("Layout")]
        [Tooltip("皿を並べるラック表示の中心位置")]
        [SerializeField] private Transform rackDisplayCenter;

        [Tooltip("ターゲットを並べる基準位置")]
        [SerializeField] private Transform targetAreaCenter;

        [Tooltip("ターゲット間の間隔")]
        [SerializeField] private float targetSpacing = 2f;

        [Header("Visuals")]
        [Tooltip("ターゲットの半透明度")]
        [SerializeField, Range(0f, 1f)] private float targetAlpha = 0.3f;

        [Tooltip("皿1セルあたりのワールドサイズ")]
        [SerializeField] private float cellSize = 1f;

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

            gameObject.SetActive(true);
            SpawnDishes(rack.PlacementData);
            SpawnTargets(rack.PlacementData);
        }

        /// <summary>
        /// ラック上の皿をD&D可能なオブジェクトとして生成する
        /// </summary>
        private void SpawnDishes(RackPlacementData data)
        {
            var rackCenter = rackDisplayCenter != null ? rackDisplayCenter.position : transform.position;

            foreach (var dish in data.Dishes)
            {
                var obj = new GameObject($"SortingDish_{dish.DishTypeKey}");
                obj.transform.SetParent(transform, false);

                // グリッド座標からワールド位置を計算
                var pos = new Vector3(
                    rackCenter.x + dish.GridOrigin.x * cellSize,
                    rackCenter.y + dish.GridOrigin.y * cellSize,
                    0f
                );
                obj.transform.position = pos;

                var sortingDish = obj.AddComponent<SortingDish>();
                sortingDish.Initialize(dish.DishTypeKey, dish.Sprite, OnDishSorted);
                _activeDishes.Add(sortingDish);
                _spawnedObjects.Add(obj);
            }
        }

        /// <summary>
        /// 種類ごとのドロップターゲットを生成する
        /// </summary>
        private void SpawnTargets(RackPlacementData data)
        {
            var targetCenter = targetAreaCenter != null ? targetAreaCenter.position : transform.position + Vector3.down * 3f;

            // 種類ごとにグループ化（最初の1つのスプライトを代表として使う）
            var typeGroups = new Dictionary<string, Sprite>();
            foreach (var dish in data.Dishes)
            {
                if (!typeGroups.ContainsKey(dish.DishTypeKey))
                {
                    typeGroups[dish.DishTypeKey] = dish.Sprite;
                }
            }

            var typeCount = typeGroups.Count;
            var startX = -(typeCount - 1) * targetSpacing / 2f;
            var index = 0;

            foreach (var kvp in typeGroups)
            {
                var obj = new GameObject($"SortingTarget_{kvp.Key}");
                obj.transform.SetParent(transform, false);
                obj.transform.position = new Vector3(
                    targetCenter.x + startX + index * targetSpacing,
                    targetCenter.y,
                    0f
                );

                var target = obj.AddComponent<SortingTarget>();
                target.Initialize(kvp.Key, kvp.Value, targetAlpha);
                _spawnedObjects.Add(obj);

                index++;
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
            gameObject.SetActive(false);
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
