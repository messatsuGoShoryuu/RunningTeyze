using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RunningTeyze.UI
{
    public class TeyzeItemUI : MonoBehaviour
    {
        [SerializeField]
        Text m_name;

        [SerializeField]
        Image m_image;

        Teyze m_teyze;

        public void SetInfo(Teyze teyze)
        {
            m_name.text = teyze.name;
            m_image.sprite = Resources.Load<Sprite>("Sprites/TeyzePortraits/" + teyze.name);
            m_teyze = teyze;
        }

        public void BtnTeyze()
        {
            Teyze oldTeyze = GameState.currentTeyze;
            Teyze newTeyze = m_teyze;

            GameState.SetCurrentTeyze(m_teyze.name);

            EventSystem.Dispatch(new Event_CurrentTeyzeChanged(newTeyze, oldTeyze));
        }
    }
}
