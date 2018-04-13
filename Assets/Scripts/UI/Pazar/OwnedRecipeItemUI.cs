using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RunningTeyze.UI
{
    public class OwnedRecipeItemUI : MonoBehaviour
    {
        [SerializeField]
        Text m_name;

        [SerializeField]
        Text m_skillPoints;

        [SerializeField]
        Text m_difficulty;

        Recipe m_recipe;

        public void SetRecipeInfo(Recipe recipe)
        {
            m_name.text = recipe.name;
            m_skillPoints.text = recipe.yieldSP.ToString() + " SP";
            m_difficulty.text = "Difficulty = " + recipe.difficultySP.ToString();
            m_recipe = recipe;
        }

        public void OnHover()
        {
            EventSystem.Dispatch(new Event_RecipeUIOnRecipeHovered(m_recipe));
        }
    }
}