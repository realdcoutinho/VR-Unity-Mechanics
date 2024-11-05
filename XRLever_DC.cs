using UnityEngine;

namespace Unity.VRTemplate
{
    /// <summary>
    /// A concrete implementation of XRBaseLever_DC, handling specific lever logic.
    /// </summary>
    public class XRLever_DC : XRBaseLever_DC
    {
        /// <summary>
        /// Updates the lever's rotation based on the interactor's position and direction.
        /// </summary>
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

            // Apply the lever's rotation
            SetKnobRotation(leverAngle);

            // Normalize the lever value between 0 and 1 based on its angle
            float knobValue = (leverAngle - m_MinAngle) / (m_MaxAngle - m_MinAngle);
            SetValue(knobValue);
        }
    }
}
