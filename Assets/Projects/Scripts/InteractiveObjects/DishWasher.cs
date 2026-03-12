using Projects.Scripts.Control;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Projects.Scripts.InteractiveObjects
{
    /// <summary>
    /// 食洗機のGameObject
    /// </summary>
    public class DishWasher : MonoBehaviour, IInputHandler
    {
        private const float WashingTime = 5.0f;

        private bool _isRunning;
        
        /// <summary>
        /// 押下として利用
        /// </summary>
        public void OnInputBegin(Vector2 pos)
        {
            if (_isRunning) return;
            WasherTimer().Forget();
        }

        private async UniTaskVoid WasherTimer()
        {
            _isRunning = true;
            var currentTime = WashingTime;

            while (currentTime >= 0)
            {
                await UniTask.Yield();
                currentTime -= Time.deltaTime;
            }

            _isRunning = false;
            Debug.Log("洗浄完了！");
        }

        public void OnInputDrag(Vector2 pos) { }

        public void OnInputEnd(Vector2 pos) { }
    }
}