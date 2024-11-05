using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEngine.XR.Content.Interaction
{
    public class XRGripButton_DC : XRBaseButton_DC
    {
        [SerializeField]
        private bool m_ToggleButton = false;

        private bool m_Toggled = false;
        private bool m_Selected = false;

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

        //protected override void StartPress()
        //{
        //    SetButtonHeight(-m_PressDistance);
        //    m_OnPress.Invoke();
        //    m_Selected = true;
        //}

        //protected override void EndPress()
        //{
        //    SetButtonHeight(0.0f);
        //    m_OnRelease.Invoke();
        //    m_Selected = false;
        //}



        protected void StartPress(SelectEnterEventArgs args)
        {
            SetButtonHeight(-m_PressDistance);
            m_OnPress.Invoke();
            m_Selected = true;
        }

        protected void EndPress(SelectExitEventArgs args)
        {
            //if (m_Hovered)

            SetButtonHeight(0.0f);
            m_OnRelease.Invoke();
            m_Selected = false;
        }

        protected override void StartPress()
        {
            throw new System.NotImplementedException();
        }

        protected override void EndPress()
        {
            throw new System.NotImplementedException();
        }
    }
}
