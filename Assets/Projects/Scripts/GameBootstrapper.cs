using Projects.Scripts.Touch;
using UnityEngine;

namespace Projects.Scripts
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private TouchableObject sampleExeObj;

        private void Awake()
        {
            var sampleExecution = new SampleExecution();
            sampleExeObj.SetTouchBehaviour(sampleExecution);
        }
    }
}