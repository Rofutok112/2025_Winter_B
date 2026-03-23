namespace Projects.Scripts.InteractiveObjects
{
    /// <summary>
    /// UI画面表示中にバックグラウンドオブジェクトのインタラクトを抑制するための静的フラグ。
    /// </summary>
    public static class InteractionLock
    {
        public static bool IsLocked { get; set; }
    }
}
