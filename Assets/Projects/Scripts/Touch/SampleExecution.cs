using UnityEngine;

namespace Projects.Scripts.Touch
{
    public class SampleExecution : IExecutableOnTouch
    {
        public void Execute()
        {
            Debug.Log($"SampleExecution is called");
        }
    }
}