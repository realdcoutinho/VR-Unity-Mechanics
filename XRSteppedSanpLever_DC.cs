using UnityEngine;

namespace Unity.VRTemplate
{
    /// <summary>
    /// A lever that stops at specific steps, based on the number of increments.
    /// Inherits from XRBaseLever_DC.
    /// </summary>
    public class XRSteppedSnapLever_DC : XRBaseLever_DC
    {
        [SerializeField]
        [Tooltip("Number of steps the lever can stop at. For example, 2 means it will only stop at the start or end.")]
        private int increments = 2; // Number of steps the lever can stop at (editable in the Editor)

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

            // Snap the angle to the nearest step based on the number of increments
            leverAngle = SnapToIncrement(leverAngle);

            // Apply the lever's rotation
            SetKnobRotation(leverAngle);

            // Normalize the lever value between 0 and 1 based on its angle
            float knobValue = (leverAngle - m_MinAngle) / (m_MaxAngle - m_MinAngle);
            SetValue(knobValue);
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

        protected override void OnValidate()
        {
            base.OnValidate();
            if (increments < 2) increments = 2; // Ensure there's at least 2 increments for snapping
        }
    }
}
