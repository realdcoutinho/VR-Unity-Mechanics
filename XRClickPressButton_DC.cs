using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Content.Interaction
{
    public class XRClickPressButton_DC : XRBaseButton_DC
    {
        [SerializeField]
        private bool m_ToggleButton = false;

        private bool m_Toggled = false;
        private IXRSelectInteractor m_GrippingInteractor = null;
        private List<IXRInteractor> m_RegisteredInteractors = new List<IXRInteractor>();

        [SerializeField]
        private XRInteractionManager interactionManager;

        private void Start()
        {
            // Find the XRInteractionManager in the scene if not assigned
            if (interactionManager == null)
            {
                interactionManager = FindObjectOfType<XRInteractionManager>();
                if (interactionManager == null)
                {
                    Debug.LogError("XRInteractionManager not found in the scene.");
                }
                else
                {
                    Debug.Log("XRInteractionManager found and assigned.");
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (m_ToggleButton)
                selectEntered.AddListener(StartTogglePress);
            else
            {
                selectEntered.AddListener(StartPress);
                selectExited.AddListener(EndPress);
            }
        }

        protected override void OnDisable()
        {
            if (m_ToggleButton)
                selectEntered.RemoveListener(StartTogglePress);
            else
            {
                selectEntered.RemoveListener(StartPress);
                selectExited.RemoveListener(EndPress);
            }
            base.OnDisable();
        }

        private void StartTogglePress(SelectEnterEventArgs args)
        {
            m_Toggled = !m_Toggled;
            SetButtonHeight(m_Toggled ? -m_PressDistance : 0.0f);
            m_OnPress.Invoke();
        }

        protected void StartPress(SelectEnterEventArgs args)
        {
            m_GrippingInteractor = args.interactorObject;
            SetButtonHeight(-m_PressDistance);
            m_OnPress.Invoke();
        }

        protected void EndPress(SelectExitEventArgs args)
        {
            m_GrippingInteractor = null;
            SetButtonHeight(0.0f);
            m_OnRelease.Invoke();
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            Debug.Log("Processing interactable.");

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                Debug.Log("Dynamic phase");

                // Check if we have a gripping interactor
                if (m_GrippingInteractor != null)
                {
                    Debug.Log("Interactor exists");
                }

                // Check for any existing grip, even if it started outside the prefab's collider
                if (CheckForExistingGrip())
                {
                    Debug.Log("Grip exists, even outside the collider");
                    // Perform button update here
                    // UpdateGripPress();
                }
            }
        }

        private bool CheckForExistingGrip()
        {
            interactionManager.GetRegisteredInteractors(m_RegisteredInteractors);

            if (m_RegisteredInteractors.Count == 0)
            {
                Debug.Log("No registered interactors found.");
            }

            foreach (var interactor in m_RegisteredInteractors)
            {
                Debug.Log($"Checking interactor: {interactor}");

                if (interactor is IXRSelectInteractor selectInteractor)
                {
                    Debug.Log($"Interactor {selectInteractor} - isSelectActive: {selectInteractor.isSelectActive}");

                    if (selectInteractor.isSelectActive)
                    {
                        Debug.Log("Interactor is actively selecting.");

                        if (selectInteractor.interactablesSelected.Count > 0)
                        {
                            Debug.Log($"Interactor is selecting {selectInteractor.interactablesSelected.Count} interactables.");
                            foreach (var interactable in selectInteractor.interactablesSelected)
                            {
                                Debug.Log($"Interactor is selecting: {interactable}");
                            }

                            // If the interactor is gripping any interactable
                            Debug.Log("Grip detected.");
                            m_GrippingInteractor = selectInteractor;
                            return true;
                        }
                        else
                        {
                            Debug.Log("Interactor is not selecting any interactables.");
                        }
                    }
                }
            }
            return false;
        }


        private void UpdateGripPress()
        {
            if (m_GrippingInteractor == null) return;

            var interactorTransform = m_GrippingInteractor.GetAttachTransform(this);
            var localOffset = transform.InverseTransformVector(interactorTransform.position - m_Button.position);

            var newHeight = Mathf.Clamp(localOffset.y, -m_PressDistance, 0.0f);
            SetButtonHeight(newHeight);

            var pressed = newHeight <= -m_PressDistance;
            if (pressed && !m_Pressed)
            {
                m_OnPress.Invoke();
            }
            else if (!pressed && m_Pressed)
            {
                m_OnRelease.Invoke();
            }
            m_Pressed = pressed;
        }

        protected override void StartPress() { throw new NotImplementedException(); }
        protected override void EndPress() { throw new NotImplementedException(); }
    }
}
