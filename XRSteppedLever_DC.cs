using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Unity.VRTemplate
{
    /// <summary>
    /// A lever that shows smooth movement during interaction but snaps to specific steps when released.
    /// Inherits from XRBaseLever_DC.
    /// </summary>
    public class XRSteppedLever_DC : XRBaseLever_DC
    {
        [SerializeField]
        [Tooltip("Number of steps the lever can stop at. For example, 2 means it will only stop at the start or end.")]
        private int increments = 2; // Number of steps the lever can stop at (editable in the Editor)

        private bool isSnapping = false;

        protected override void UpdateRotation()
        {
            if (m_Interactor == null) return;

            // Calculate the direction from the interactor (controller/hand) to the lever
            Vector3 interactorDirection = m_Interactor.GetAttachTransform(this).position - m_Handle.position;
            interactorDirection = transform.InverseTransformDirection(interactorDirection);
            interactorDirection.x = 0; // Ignore X-axis for input

            float leverAngle = Mathf.Atan2(interactorDirection.z, interactorDirection.y) * Mathf.Rad2Deg;

            // Clamp the angle between min and max limits
            leverAngle = Mathf.Clamp(leverAngle, m_MinAngle, m_MaxAngle);

            // While interacting, show the smooth movement of the lever without snapping
            SetKnobRotation(leverAngle);

            // Normalize the lever value between 0 and 1 based on its angle
            float knobValue = (leverAngle - m_MinAngle) / (m_MaxAngle - m_MinAngle);
            SetValue(knobValue);
        }

        /// <summary>
        /// Called when the interactor stops grabbing the lever.
        /// Snaps the lever to the nearest increment.
        /// </summary>
        protected override void EndGrab(SelectExitEventArgs args)
        {
            base.EndGrab(args);
            float snappedAngle = SnapToIncrement(m_Handle.localEulerAngles.x); // Snap the lever angle
            StartCoroutine(SnapToAngle(snappedAngle)); // Smooth snapping effect
        }

        /// <summary>
        /// Snaps the given angle to the nearest increment step.
        /// </summary>
        /// <param name="angle">The current lever angle.</param>
        /// <returns>The snapped angle.</returns>
        private float SnapToIncrement(float angle)
        {
            if (increments <= 1)
                return angle; // No snapping if increments are 1 or less

            // Calculate the size of each increment step
            float stepSize = (m_MaxAngle - m_MinAngle) / (increments - 1);

            // Calculate the nearest step based on the current angle
            float snappedAngle = Mathf.Round((angle - m_MinAngle) / stepSize) * stepSize + m_MinAngle;

            return snappedAngle;
        }

        /// <summary>
        /// Smoothly snaps the lever to the specified angle after releasing the interactor.
        /// </summary>
        /// <param name="targetAngle">The target angle to snap to.</param>
        private System.Collections.IEnumerator SnapToAngle(float targetAngle)
        {
            isSnapping = true;
            float currentAngle = m_Handle.localEulerAngles.x;
            float duration = 0.2f; // Time taken to complete the snap
            float elapsedTime = 0.0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float newAngle = Mathf.Lerp(currentAngle, targetAngle, elapsedTime / duration);
                SetKnobRotation(newAngle);
                yield return null;
            }

            SetKnobRotation(targetAngle); // Ensure it finishes exactly at the target
            isSnapping = false;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (increments < 2) increments = 2; // Ensure there's at least 2 increments for snapping
        }
    }
}
