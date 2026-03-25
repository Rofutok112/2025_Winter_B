using Projects.Scripts.InteractiveObjects;
using UnityEngine;

namespace Projects.Scripts.Audio
{
    public class DishWasherAudioBinder : MonoBehaviour
    {
        [SerializeField] private DishWasher dishWasher;
        [SerializeField] private DishWasherAudioPresenter audioPresenter;

        private void OnEnable()
        {
            if (!ValidateReferences())
            {
                return;
            }

            dishWasher.WashStarted += audioPresenter.HandleWashStarted;
            dishWasher.WashCompleted += audioPresenter.HandleWashCompleted;
            dishWasher.StateChanged += audioPresenter.HandleStateChanged;
        }

        private void OnDisable()
        {
            if (dishWasher == null || audioPresenter == null)
            {
                return;
            }

            dishWasher.WashStarted -= audioPresenter.HandleWashStarted;
            dishWasher.WashCompleted -= audioPresenter.HandleWashCompleted;
            dishWasher.StateChanged -= audioPresenter.HandleStateChanged;
        }

        private bool ValidateReferences()
        {
            if (dishWasher == null || audioPresenter == null)
            {
                Debug.LogWarning($"{nameof(DishWasherAudioBinder)} is missing references.", this);
                return false;
            }

            return true;
        }
    }
}
