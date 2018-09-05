using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RunningTeyze.UI
{
    public class CookUI : MonoBehaviour
    {
        [SerializeField]
        GameObject m_ownedRecipies;
        [SerializeField]
        GameObject m_recipeIngredients;

        [SerializeField]
        Text m_recipeNameText;

        [SerializeField]
        string m_ownedRecipeItem;

        [SerializeField]
        string m_recipeIngredientItem;

        Recipe m_selectedRecipe;

        private void OnEnable()
        {
            Recipe[] recipes = GameState.currentTeyze.recipes;

            for (int i = 0; i < recipes.Length; i++)
            {
                addRecipe(recipes[i]);
            }


            EventSystem.AddListener<Event_RecipeCooked>(OnCook);
            EventSystem.AddListener<Event_RecipeUIOnRecipeHovered>(OnRecipeHovered);
            EventSystem.AddListener<Event_CurrentTeyzeChanged>(OnTeyzeChanged);
        }

        void OnTeyzeChanged(Event_CurrentTeyzeChanged e)
        {
            Utilities.DestroyTransformChildren(m_ownedRecipies.transform);
            Utilities.DestroyTransformChildren(m_recipeIngredients.transform);

            Recipe[] recipes = GameState.currentTeyze.recipes;

            for (int i = 0; i < recipes.Length; i++)
            {
                addRecipe(recipes[i]);
            }
        }

        private void OnDisable()
        {
            Utilities.DestroyTransformChildren(m_ownedRecipies.transform);
            Utilities.DestroyTransformChildren(m_recipeIngredients.transform);

            EventSystem.RemoveListener<Event_RecipeCooked>(OnCook);
            EventSystem.RemoveListener<Event_RecipeUIOnRecipeHovered>(OnRecipeHovered);
            EventSystem.RemoveListener<Event_CurrentTeyzeChanged>(OnTeyzeChanged);
        }

       void OnCook(Event_RecipeCooked e)
        {
            Debug.Log("Cooking recipe : " + e.recipe.name);
        }

        public void BtnCook()
        {
            if(m_selectedRecipe != null)
            {
                Teyze.Cook(m_selectedRecipe);
            }
        }

        void OnRecipeHovered(Event_RecipeUIOnRecipeHovered e)
        {
            m_selectedRecipe = e.recipe;
            m_recipeNameText.text = e.recipe.name;

            Utilities.DestroyTransformChildren(m_recipeIngredients.transform);

            
            for (int i = 0; i < e.recipe.ingredients.Length; i++)
                addRecipeIngredient(e.recipe.ingredients[i]);
        }

        void addRecipe(Recipe recipe)
        {
            GameObject go = Utilities.InstantiateFromResources("Prefabs/UI/Pazar/" + m_ownedRecipeItem);
            go.transform.SetParent(m_ownedRecipies.transform);
            go.GetComponent<OwnedRecipeItemUI>().SetRecipeInfo(recipe);
        }

        void addRecipeIngredient(IngredientInstance ing)
        {
            GameObject go = Utilities.InstantiateFromResources("Prefabs/UI/Pazar/" + m_recipeIngredientItem);
            go.transform.SetParent(m_recipeIngredients.transform);

            OwnedIngredientItemUI r = go.GetComponent<OwnedIngredientItemUI>();
            r.SetInfo(ing);
        }

    }
}
