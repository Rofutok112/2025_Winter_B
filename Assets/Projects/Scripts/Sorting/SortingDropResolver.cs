using System.Collections.Generic;
using UnityEngine;

namespace Projects.Scripts.Sorting
{
    public sealed class SortingDropResolver
    {
        private readonly IReadOnlyList<SortingTarget> _targets;
        private readonly float _targetRadius;

        public SortingDropResolver(IReadOnlyList<SortingTarget> targets, float targetRadius)
        {
            _targets = targets;
            _targetRadius = targetRadius;
        }

        public SortingTarget FindClosestTarget(Vector2 worldPos)
        {
            if (_targets == null) return null;

            SortingTarget closest = null;
            var closestDist = float.MaxValue;

            foreach (var target in _targets)
            {
                if (target == null) continue;

                var dist = Vector2.Distance(worldPos, target.transform.position);
                if (dist < closestDist && dist <= _targetRadius)
                {
                    closestDist = dist;
                    closest = target;
                }
            }

            return closest;
        }
    }
}
