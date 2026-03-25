using Cysharp.Threading.Tasks;

namespace Projects.Scripts.UI
{
    public interface IGameResultAnimationDriver
    {
        void ResetVisuals(GameResultAnimationTargets targets);
        UniTask PlayShowAsync(GameResultAnimationTargets targets);
    }
}
