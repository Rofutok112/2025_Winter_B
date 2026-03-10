using UnityEngine;
using UnityEngine.InputSystem;

namespace Projects.Scripts.Touch
{
    public class TouchableObject : MonoBehaviour
    {
        private IExecutableOnTouch _touchBehaviour;
        
        /// <summary>
        /// 固有のアクションを設定する
        /// </summary>
        /// <param name="touchBehaviour"></param>
        public void SetTouchBehaviour(IExecutableOnTouch touchBehaviour)
        {
            _touchBehaviour = touchBehaviour;
        }

        /// <summary>
        /// 固有のアクションを実行する
        /// </summary>
        public void Execute()
        {
            _touchBehaviour?.Execute();
        }
    }
}
