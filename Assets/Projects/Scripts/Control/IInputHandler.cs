using UnityEngine;

namespace Projects.Scripts.Control
{
    /// <summary>
    /// 2Dオブジェクトに対してタップやクリックでのインタラクトを可能にする。
    /// 引数Vector2はワールド座標を返す
    /// </summary>
    public interface IInputHandler
    {
        /// <summary>
        /// オブジェクトがクリックされたときに呼ばれる
        /// </summary>
        void OnInputBegin(Vector2 pos);
        
        /// <summary>
        /// このオブジェクトがドラッグされている間、毎フレーム呼ばれる
        /// </summary>
        void OnInputDrag(Vector2 pos);
        
        /// <summary>
        /// このオブジェクトがクリックを終了したときに呼ばれる
        /// </summary>
        void OnInputEnd(Vector2 pos);
    }
}