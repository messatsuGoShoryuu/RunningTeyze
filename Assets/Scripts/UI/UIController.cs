using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        InputMapper m_pazarUI;
        [SerializeField]
        Transform m_pazarContent;
        [SerializeField]
        Transform m_pazarInventoryContent;


        // Use this for initialization
        void Awake()
        {
            EventSystem.AddListener<Event_PazarShowUI>(OnPazarShowUI);
            EventSystem.AddListener<Event_PlayerIngredientBought>(OnPlayerIngredientBuy);
        }

        void OnPlayerIngredientBuy(Event_PlayerIngredientBought e)
        {
            int count = m_pazarInventoryContent.childCount;

            bool found = false;
            for(int i = 0; i<count;i++)
            {
                if(m_pazarInventoryContent.GetChild(i).name == e.ingredient.name)
                {
                    found = true;
                    PazarItemUI p = m_pazarInventoryContent.GetChild(i).GetComponent<PazarItemUI>();
                    p.addAmount(e.kg);
                }
            }
            if(!found)
            {
                PazarItemUI item = PazarItemUI.Create(e.ingredient,
                       0.0f, e.kg);
                item.name = e.ingredient.name;
                item.transform.SetParent(m_pazarInventoryContent);
            }
        }

        void OnPazarShowUI(Event_PazarShowUI e)
        {
            m_pazarUI.Enable();

            int n = m_pazarContent.childCount;
            for (int i = 0; i < n; i++)
                GameObject.Destroy(m_pazarContent.GetChild(i).transform.gameObject);

            n = m_pazarInventoryContent.childCount;
            for (int i = 0; i < n; i++)
                GameObject.Destroy(m_pazarInventoryContent.GetChild(i).transform.gameObject);

            for (int i = 0; i<e.pazar.ingredients.Count;i++)
            {
                PazarItemUI item = PazarItemUI.Create(e.pazar.ingredients[i],e.pazar.costMultiplier);
                item.transform.SetParent(m_pazarContent);
            }

            for(int i = 0; i<GameState.currentTeyze.ingredients.Length;i++)
            {
                PazarItemUI item = PazarItemUI.Create(GameState.currentTeyze.ingredients[i].ingredient,
                    e.pazar.costMultiplier,GameState.currentTeyze.ingredients[i].amountKg);
                item.transform.SetParent(m_pazarInventoryContent);
                item.name = GameState.currentTeyze.ingredients[i].ingredient.name;
            }
        }

        private void OnDestroy()
        {
            EventSystem.RemoveListener<Event_PazarShowUI>(OnPazarShowUI);
            EventSystem.RemoveListener<Event_PlayerIngredientBought>(OnPlayerIngredientBuy);
        }
    }
}
