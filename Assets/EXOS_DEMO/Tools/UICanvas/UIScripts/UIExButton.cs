using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace exiii.Unity.Develop
{
    public class UIExButton : Button
    {
        [SerializeField]
        [FormerlySerializedAs("Image")]
        private Image m_Image;

        [SerializeField]
        [FormerlySerializedAs("IconNormal")]
        private Sprite m_IconNormal;

        [SerializeField]
        [FormerlySerializedAs("IconHighlighted")]
        private Sprite m_IconHighlighted;

        [SerializeField]
        [FormerlySerializedAs("IconPressed")]
        private Sprite m_IconPressed;

        [SerializeField]
        [FormerlySerializedAs("IconDisabled")]
        private Sprite m_IconDisabled;

        private SelectionState m_SelectionState = SelectionState.Normal;

        public Sprite IconPressed { get { return this.m_IconPressed; } }

        // on update.
        public void Update()
        {
            // update sprite.
            UpdateSprite();
        }

        // update sprite.
        private void UpdateSprite()
        {
            if (currentSelectionState == m_SelectionState) { return; }

            switch (currentSelectionState)
            {
                case SelectionState.Normal:
                    m_Image.sprite = m_IconNormal;
                    break;
                case SelectionState.Highlighted:
                    m_Image.sprite = m_IconHighlighted;
                    break;
                case SelectionState.Pressed:
                    m_Image.sprite = m_IconPressed;
                    break;
                case SelectionState.Disabled:
                    m_Image.sprite = m_IconDisabled;
                    break;
            }

            // update current state.
            m_SelectionState = currentSelectionState;
        }

        // change pressed state once.
        public void ChangePressedState()
        {
            m_Image.overrideSprite = m_IconPressed;
            m_SelectionState = SelectionState.Pressed;
        }

        // reset pressed state.
        public void ResetPressedState()
        {
            m_Image.overrideSprite = null;
            m_Image.sprite = m_IconNormal;
            m_SelectionState = SelectionState.Normal;
        }
    }
}
