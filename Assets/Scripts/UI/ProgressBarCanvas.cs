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
        float m_lerpSpeed;
        float m_targetValue;

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

        private void OnEnable()
        {
            if(m_image == null)
            {
                m_image = GetComponent<RawImage>();
                m_image.material = new Material(m_image.material);
                m_image.material.SetTexture("_CutoutTex", m_cutout.texture);
                SetCutoutValue(m_cutoutValue);
            }
        }



        public override void SetCutoutValue(float value, bool ignoreLerp = false)
        {
            m_targetValue = value;
            if (m_lerpSpeed > 0.0f && !ignoreLerp)
                StartCoroutine(CR_Lerp(m_cutoutValue, value > m_cutoutValue));
            else
            {
                m_cutoutValue = value;
                if (m_image.materialForRendering != null)
                    if (m_image.materialForRendering != null)
                        m_image.materialForRendering.SetFloat("_CutoutValue", 1.0f - m_cutoutValue);
            }
        }

        IEnumerator CR_Lerp(float value, bool bigger)
        {
            yield return null;
            value = Mathf.Lerp(value, m_targetValue, m_lerpSpeed * Time.deltaTime);
            if (bigger)
                if (value > m_targetValue)
                {
                    m_cutoutValue = Mathf.Clamp01(m_targetValue);
                    m_image.materialForRendering.SetFloat("_CutoutValue", 1.0f - m_cutoutValue);
                }
                else
                {
                    value = Mathf.Clamp01(value);
                    m_cutoutValue = Mathf.Clamp01(value);
                    m_image.materialForRendering.SetFloat("_CutoutValue", 1.0f - m_cutoutValue);
                    StartCoroutine(CR_Lerp(value, bigger));
                }
            else if (value < m_targetValue)
            {
                m_cutoutValue = Mathf.Clamp01(m_targetValue);
                m_image.materialForRendering.SetFloat("_CutoutValue", 1.0f - m_cutoutValue);
            }
            else
            {
                value = Mathf.Clamp01(value);
                m_cutoutValue = Mathf.Clamp01(value);
                m_image.materialForRendering.SetFloat("_CutoutValue", 1.0f - m_cutoutValue);
                StartCoroutine(CR_Lerp(value, bigger));
            }


        }
    }
}
