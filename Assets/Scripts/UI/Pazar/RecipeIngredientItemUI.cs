using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RunningTeyze.UI
{
    public class RecipeIngredientItemUI : MonoBehaviour
    {
        [SerializeField]
        Text m_name;
        [SerializeField]
        Text m_amountKG;

        IngredientInstance m_ingredient;
        public void SetInfo(IngredientInstance ing)
        {
            m_name.text = ing.ingredient.name;
            m_amountKG.text = ing.amountKg.ToString() + " kg.";
            m_ingredient = ing;
        }

        public void BtnAddToShoppingList()
        {
            EventSystem.Dispatch(new Event_AddToShoppingList(m_ingredient.ingredient.name));
        }
        
 
    }
}
