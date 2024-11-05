using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Unity.VRTemplate
{
    /// <summary>
    /// A lever that can be moved backward or downward but returns to its original position
    /// taking the shortest path when released.
    /// Inherits from XRBaseLever_DC.
    /// </summary>
    public class XRReturningLever_DC : XRBaseLever_DC
    {
        private float originalAngle; // To store the initial angle of the lever
        private bool isReturning = false; // To track if the lever is currently returning

        // Define Start without the override keyword
        protected void Start()
        {
            originalAngle = ValueToRotation(); // Store the initial angle of the lever
        }

        protected override void UpdateRotation()
        {
            if (m_Interactor == null || isReturning) return; // Don't allow interaction if returning to position

            // Calculate the direction from the interactor (controller/hand) to the lever
            Vector3 interactorDirection = m_Interactor.GetAttachTransform(this).position - m_Handle.position;
            interactorDirection = transform.InverseTransformDirection(interactorDirection);
            interactorDirection.x = 0; // Ignore X-axis for input

            float leverAngle = Mathf.Atan2(interactorDirection.z, interactorDirection.y) * Mathf.Rad2Deg;

            // Clamp the angle between min and max limits
            leverAngle = Mathf.Clamp(leverAngle, m_MinAngle, m_MaxAngle);

            // Apply the lever's rotation based on interactor's input
            SetKnobRotation(leverAngle);

            // Normalize the lever value between 0 and 1 based on its angle
            float knobValue = (leverAngle - m_MinAngle) / (m_MaxAngle - m_MinAngle);
            SetValue(knobValue);
        }

        /// <summary>
        /// Called when the interactor stops grabbing the lever.
        /// Starts returning the lever to its original position.
        /// </summary>
        protected override void EndGrab(SelectExitEventArgs args)
        {
            base.EndGrab(args);
            StartCoroutine(ReturnToOriginalPosition());
        }

        /// <summary>
        /// Coroutine to return the lever to its original position smoothly after releasing,
        /// taking the shortest path.
        /// </summary>
        /// <returns></returns>
        private System.Collections.IEnumerator ReturnToOriginalPosition()
        {
            isReturning = true;
            float currentAngle = m_Handle.localEulerAngles.x;
            float duration = 0.3f; // Time taken to return to the original position
            float elapsedTime = 0.0f;

            // Calculate the shortest angular distance
            float shortestAngle = ShortestAngleDistance(currentAngle, originalAngle);

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float newAngle = Mathf.Lerp(currentAngle, currentAngle + shortestAngle, elapsedTime / duration);
                SetKnobRotation(newAngle);
                yield return null;
            }

            SetKnobRotation(originalAngle); // Ensure it finishes exactly at the original position
            SetValue(0f); // Reset value to 0 (or original value as needed)
            isReturning = false;
        }

        /// <summary>
        /// Calculate the shortest angular distance between two angles.
        /// </summary>
        /// <param name="fromAngle">The starting angle.</param>
        /// <param name="toAngle">The target angle.</param>
        /// <returns>The shortest angle difference.</returns>
        private float ShortestAngleDistance(float fromAngle, float toAngle)
        {
            float difference = Mathf.DeltaAngle(fromAngle, toAngle);
            return difference;
        }

        /// <summary>
        /// Optional validation in the editor.
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();
            originalAngle = ValueToRotation(); // Recalculate the original position on validation
        }
    }
}
