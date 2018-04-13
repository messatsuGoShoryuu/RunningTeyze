using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RunningTeyze
{
    public class PazarItemUI : MonoBehaviour
    {
        [SerializeField]
        Text m_name;
        [SerializeField]
        Text m_cost;
        [SerializeField]
        Text m_props;

        Ingredient m_ingredient;
        float m_costModifier;
        float m_amount = -10.0f;

        public static PazarItemUI Create(Ingredient ingredient, float costModifier, float amount)
        {
            GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/Pazar/PazarInventoryIngredientItem"));

            PazarItemUI item = go.GetComponent<PazarItemUI>();

            Debug.Assert(item != null);

            item.m_ingredient = ingredient;
            item.m_costModifier = costModifier;
            item.m_amount = amount;

            return item;
        }
        public static PazarItemUI Create(Ingredient ingredient, float costModifier)
        {
            GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/Pazar/IngredientItem"));

            PazarItemUI item = go.GetComponent<PazarItemUI>();

            Debug.Assert(item != null);

            item.m_ingredient = ingredient;
            item.m_costModifier = costModifier;
            
            return item;
        }

        public void addAmount(float amount)
        {
            m_amount += amount;
            m_cost.text = m_amount.ToString() + " kg";
        }
        
        // Use this for initialization
        void Start()
        {
            m_name.text = m_ingredient.name;
            if (m_amount < 0.0f)
            {
                m_cost.text = (m_ingredient.cost * m_costModifier).ToString();
            }
            else m_cost.text = m_amount.ToString() + " kg";

            m_props.text = "Rarity = " + m_ingredient.rarity.ToString();
            m_props.text += "\nSkill points Per KG " + m_ingredient.spPerKg.ToString();
        }

        public void BtnBuy(float amount)
        {
            EventSystem.Dispatch(new Event_PlayerIngredientBuyRequest(m_ingredient.name, amount));
        }
    }
}
