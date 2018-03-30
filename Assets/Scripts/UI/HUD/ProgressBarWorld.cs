using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze.UI.HUD
{
    public class ProgressBarWorld : ProgressBar
    {
        private SpriteRenderer m_renderer;
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
            m_renderer = GetComponent<SpriteRenderer>();
            m_renderer.material = new Material(m_renderer.material);
            m_renderer.material.SetTexture("_CutoutTex", m_cutout.texture);
            SetCutoutValue(m_cutoutValue, true);
        }

        public override void SetCutoutValue(float value, bool ignoreLerp = false)
        {
            m_targetValue = value;
            if (m_lerpSpeed > 0.0f && !ignoreLerp)
                StartCoroutine(CR_Lerp(m_cutoutValue, value > m_cutoutValue));
            else 
            {
                m_cutoutValue = value;
                if(m_renderer != null)
                    if(m_renderer.material != null)
                    m_renderer.material.SetFloat("_CutoutValue", 1.0f - m_cutoutValue);
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
                    m_renderer.material.SetFloat("_CutoutValue", 1.0f - m_cutoutValue);
                }
                else
                {
                    value = Mathf.Clamp01(value);
                    m_cutoutValue = Mathf.Clamp01(value);
                    m_renderer.material.SetFloat("_CutoutValue", 1.0f - m_cutoutValue);
                    StartCoroutine(CR_Lerp(value, bigger));
                }
            else if(value < m_targetValue)
                { 
                    m_cutoutValue = Mathf.Clamp01(m_targetValue);
                    m_renderer.material.SetFloat("_CutoutValue", 1.0f - m_cutoutValue);
                }
            else
                {
                    value = Mathf.Clamp01(value);
                    m_cutoutValue = Mathf.Clamp01(value);
                    m_renderer.material.SetFloat("_CutoutValue", 1.0f - m_cutoutValue);
                    StartCoroutine(CR_Lerp(value, bigger));
                }
             
            
        }
    }
}
