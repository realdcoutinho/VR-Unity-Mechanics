using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Unity.VRTemplate
{
    /// <summary>
    /// Abstract class that defines the basic mechanics of an XR lever.
    /// Derived classes will implement specific functionality.
    /// </summary>
    public abstract class XRBaseLever_DC : XRBaseInteractable
    {
        [Serializable]
        public class ValueChangeEvent : UnityEvent<float> { }

        [SerializeField]
        protected Transform m_Handle = null;

        [SerializeField]
        protected Transform m_Grip = null;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        protected float m_Value = 0.5f;

        [SerializeField]
        protected bool m_ClampedMotion = true;

        [SerializeField]
        protected float m_MaxAngle = 70.0f;

        [SerializeField]
        protected float m_MinAngle = -70.0f;

        [SerializeField]
        protected float m_MaxDistance = 0.17f; // Maximum allowed distance between interactor and lever

        [SerializeField]
        protected ValueChangeEvent m_OnValueChange = new ValueChangeEvent();

        protected IXRSelectInteractor m_Interactor;

        // Allow derived classes to set custom handle and angle limits
        public Transform handle
        {
            get => m_Handle;
            set => m_Handle = value;
        }

        public float value
        {
            get => m_Value;
            set
            {
                SetValue(value);
                SetKnobRotation(ValueToRotation());
            }
        }

        public bool clampedMotion
        {
            get => m_ClampedMotion;
            set => m_ClampedMotion = value;
        }

        public float maxAngle
        {
            get => m_MaxAngle;
            set => m_MaxAngle = value;
        }

        public float minAngle
        {
            get => m_MinAngle;
            set => m_MinAngle = value;
        }

        public float maxDistance
        {
            get => m_MaxDistance;
            set => m_MaxDistance = value;
        }

        public ValueChangeEvent onValueChange => m_OnValueChange;

        // Core functionality for setting the lever's rotation
        protected virtual void SetKnobRotation(float angle)
        {
            if (m_Handle != null)
            {
                m_Handle.localRotation = Quaternion.Euler(angle, 0.0f, 0.0f);  // Only rotate on the X-axis
            }
        }

        // Core functionality for handling interaction start
        protected virtual void StartGrab(SelectEnterEventArgs args)
        {
            m_Interactor = args.interactorObject;
        }

        // Core functionality for handling interaction end
        protected virtual void EndGrab(SelectExitEventArgs args)
        {
            m_Interactor = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            selectEntered.AddListener(StartGrab);
            selectExited.AddListener(EndGrab);
        }

        protected override void OnDisable()
        {
            selectEntered.RemoveListener(StartGrab);
            selectExited.RemoveListener(EndGrab);
            base.OnDisable();
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && isSelected && m_Interactor != null)
            {
                // Calculate the distance between the interactor and the handle
                float distance = Vector3.Distance(m_Interactor.transform.position, m_Grip.position);
                //Debug.Log($"Distance: {distance}");

                // Check if the distance exceeds the maximum allowed distance
                if (distance > m_MaxDistance)
                {
                    DetachInteractor();
                    return;
                }

                // Otherwise, update the rotation
                UpdateRotation();
            }
        }

        // Method to detach the interactor
        private void DetachInteractor()
        {
            if (m_Interactor != null)
            {
                interactionManager.SelectExit(m_Interactor, this);
                m_Interactor = null;
            }
        }

        // Override to make interactable grabbable again
        public override bool IsSelectableBy(IXRSelectInteractor interactor)
        {
            // Ensure the interactor can select if it's within range
            float distance = Vector3.Distance(interactor.transform.position, m_Grip.position);
            return base.IsSelectableBy(interactor) && distance <= m_MaxDistance;
        }

        // Abstract method to allow specific lever mechanics to be defined in derived classes
        protected abstract void UpdateRotation();

        protected void SetValue(float newValue)
        {
            if (m_ClampedMotion)
                newValue = Mathf.Clamp01(newValue);

            m_Value = newValue;
            m_OnValueChange.Invoke(m_Value);
        }

        protected float ValueToRotation()
        {
            return Mathf.Lerp(m_MinAngle, m_MaxAngle, m_Value);
        }

        // Optional validation in the editor
        protected virtual void OnValidate()
        {
            SetKnobRotation(ValueToRotation());
        }
    }
}
