using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RunningTeyze.UI
{
    public class OwnedIngredientItemUI : MonoBehaviour
    {
        [SerializeField]
        Text m_name;

        [SerializeField]
        Text m_yieldSP;

        [SerializeField]
        Text m_amount;


        IngredientInstance m_instance;

        public  void SetInfo(IngredientInstance instance)
        {
            m_name.text = instance.ingredient.name;
            m_yieldSP.text = (instance.ingredient.spPerKg * instance.amountKg).ToString() + " SP.";
            m_amount.text = instance.amountKg.ToString() + " kg.";
            m_instance = instance;
        }

        public void BtnAddToShoppingList()
        {
            EventSystem.Dispatch(new Event_AddToShoppingList(m_instance.ingredient.name));
        }
    }
}
