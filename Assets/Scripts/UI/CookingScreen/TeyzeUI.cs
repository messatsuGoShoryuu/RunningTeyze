using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze.UI
{
    public class TeyzeUI : MonoBehaviour
    {
        [SerializeField]
        Transform m_content;

        // Use this for initialization
        void Start()
        {
            for(int i = 0; i<GameState.ownedTeyzes.Length;i++)
            {
                GameObject go = Utilities.InstantiateFromResources("Prefabs/UI/CookingUI/TeyzeItem");
                TeyzeItemUI item = go.GetComponent<TeyzeItemUI>();
                item.SetInfo(GameState.ownedTeyzes[i]);
                go.transform.SetParent(m_content);
                go.transform.localScale = Vector3.one;
            }
        }
    }
}
