using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Projects.Scripts.Touch
{
    public class TouchManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        
        private void Update()
        {
            var touchscreen = Touchscreen.current;

            if (touchscreen == null) return;
            var primaryTouch = touchscreen.primaryTouch;

            if (!primaryTouch.press.wasPressedThisFrame) return;
            
            // タッチした座標を取得
            var touchScreenPosition = primaryTouch.position.ReadValue();
            var cameraRay = mainCamera.ScreenPointToRay(touchScreenPosition);
            
            // タッチした座標上のオブジェクトを取得
            if (!Physics.Raycast(cameraRay, out var raycastHit)) return;

            if (!raycastHit.collider
                .TryGetComponent<TouchableObject>(out var obj))
                return;
            
            obj.Execute();
        }
    }
}