using Projects.Scripts.UI;
using UnityEngine;

namespace Projects.Scripts.InteractiveObjects
{
    public class RackManagerUiBinder : MonoBehaviour
    {
        [SerializeField] private RackManager rackManager;
        [SerializeField] private UiActivator puzzleConfirmedActivator;
        [SerializeField] private UiActivator puzzleWindowActivatedActivator;

        private void OnEnable()
        {
            if (!ValidateReferences())
            {
                return;
            }

            rackManager.PuzzleConfirmed += puzzleConfirmedActivator.Deactivate;
            rackManager.PuzzleWindowActivated += puzzleWindowActivatedActivator.Activate;
        }

        private void OnDisable()
        {
            if (rackManager == null)
            {
                return;
            }

            if (puzzleConfirmedActivator != null)
            {
                rackManager.PuzzleConfirmed -= puzzleConfirmedActivator.Deactivate;
            }

            if (puzzleWindowActivatedActivator != null)
            {
                rackManager.PuzzleWindowActivated -= puzzleWindowActivatedActivator.Activate;
            }
        }

        private bool ValidateReferences()
        {
            if (rackManager == null ||
                puzzleConfirmedActivator == null ||
                puzzleWindowActivatedActivator == null)
            {
                Debug.LogWarning($"{nameof(RackManagerUiBinder)} is missing references.", this);
                return false;
            }

            return true;
        }
    }
}
