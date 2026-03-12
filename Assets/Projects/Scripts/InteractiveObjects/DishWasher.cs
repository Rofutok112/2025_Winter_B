using Projects.Scripts.Control;
using UnityEngine;

namespace Projects.Scripts.InteractiveObjects
{
    /// <summary>
    /// 食洗機のGameObject
    /// </summary>
    public class DishWasher : MonoBehaviour, IInputHandler
    {
        private const float WashingTime = 5.0f;
        
        /// <summary>
        /// 押下として利用
        /// </summary>
        public void OnInputBegin(Vector2 pos)
        {
            
        }

        public void OnInputDrag(Vector2 pos) { }

        public void OnInputEnd(Vector2 pos) { }
    }
}