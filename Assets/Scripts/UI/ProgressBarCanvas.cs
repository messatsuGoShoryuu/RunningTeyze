using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RunningTeyze.UI
{
    public class ProgressBarCanvas : ProgressBar
    {
        private RawImage m_image;
        [SerializeField]
        Sprite m_cutout;

        [SerializeField]
        float m_cutoutValue;
        public float cutoutValue { get { return m_cutoutValue; } }

        // Use this for initialization
        void Start()
        {
            m_image = GetComponent<RawImage>();
            m_image.material = new Material(m_image.material);
            m_image.material.SetTexture("_CutoutTex", m_cutout.texture);
            SetCutoutValue(m_cutoutValue);
        }

        public override void SetCutoutValue(float value, bool ignoreLerp = false)
        {           
            m_cutoutValue = Mathf.Clamp01(value);
            m_image.materialForRendering.SetFloat("_CutoutValue", 1.0f - value);
        }
    }
}
