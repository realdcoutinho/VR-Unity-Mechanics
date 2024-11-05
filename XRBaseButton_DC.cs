using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace UnityEngine.XR.Content.Interaction
{
    public abstract class XRBaseButton_DC : XRBaseInteractable
    {
        [SerializeField]
        protected Transform m_Button = null;

        [SerializeField]
        protected float m_PressDistance = 0.1f;

        [SerializeField]
        protected UnityEvent m_OnPress;

        [SerializeField]
        protected UnityEvent m_OnRelease;

        protected bool m_Pressed = false;

        public Transform button
        {
            get => m_Button;
            set => m_Button = value;
        }

        public float pressDistance
        {
            get => m_PressDistance;
            set => m_PressDistance = value;
        }

        public UnityEvent onPress => m_OnPress;
        public UnityEvent onRelease => m_OnRelease;

        protected virtual void SetButtonHeight(float height)
        {
            if (m_Button == null)
                return;

            Vector3 newPosition = m_Button.localPosition;
            newPosition.y = height;
            m_Button.localPosition = newPosition;
        }

        protected virtual void OnValidate()
        {
            SetButtonHeight(0.0f);
        }

        protected abstract void StartPress();
        protected abstract void EndPress();
    }
}
