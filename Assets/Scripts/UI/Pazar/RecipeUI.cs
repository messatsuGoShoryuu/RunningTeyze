using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze.UI
{
    public class RecipeUI : MonoBehaviour
    {
        [SerializeField]
        GameObject m_ownedRecipies;
        [SerializeField]
        GameObject m_recipeIngredients;
        [SerializeField]
        GameObject m_ownedIngredients;
        [SerializeField]
        GameObject m_shoppingList;
        [SerializeField]

        string m_ownedRecipeItem;
        [SerializeField]
        string m_shoppingListItem;
        [SerializeField]
        string m_ownedIngredientItem;
        [SerializeField]
        string m_recipeIngredientItem;


        private void OnDisable()
        {
            Utilities.DestroyTransformChildren(m_ownedIngredients.transform);
            Utilities.DestroyTransformChildren(m_ownedRecipies.transform);
            Utilities.DestroyTransformChildren(m_shoppingList.transform);
            EventSystem.RemoveListener<Event_RecipeUIOnRecipeHovered>(OnRecipeHovered);
            EventSystem.RemoveListener<Event_AddToShoppingList>(OnAddedToShoppingList);
        }
        private void OnEnable()
        {
            EventSystem.AddListener<Event_RecipeUIOnRecipeHovered>(OnRecipeHovered);
            EventSystem.AddListener<Event_AddToShoppingList>(OnAddedToShoppingList);
            Enable();
        }

        void OnAddedToShoppingList(Event_AddToShoppingList e)
        {
            addToShoppingList(e.name, true);
        }
        
        void OnRecipeHovered(Event_RecipeUIOnRecipeHovered e)
        {
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

            RecipeIngredientItemUI r = go.GetComponent<RecipeIngredientItemUI>();
            r.SetInfo(ing);
        }

        void addToShoppingList(string name, bool checkIfExsists)
        {
            if(checkIfExsists)
                if (ShoppingListItem.Exists(name)) return;
            
            GameObject go = Utilities.InstantiateFromResources("Prefabs/UI/Pazar/" + m_shoppingListItem) ;
            go.transform.SetParent(m_shoppingList.transform);
            ShoppingListItem item = go.GetComponent<ShoppingListItem>();
            item.SetInfo(name);
        }

        void addOwnedIngredient(IngredientInstance ingredient)
        {
            GameObject go = Utilities.InstantiateFromResources("Prefabs/UI/Pazar/" + m_ownedIngredientItem);
            go.transform.SetParent(m_ownedIngredients.transform);

            OwnedIngredientItemUI  item = go.GetComponent<OwnedIngredientItemUI>();
            item.SetInfo(ingredient);
        }

        void OnDestroy()
        {
            Utilities.DestroyTransformChildren(m_recipeIngredients.transform);
            Utilities.DestroyTransformChildren(m_ownedIngredients.transform);
            Utilities.DestroyTransformChildren(m_ownedRecipies.transform);
            Utilities.DestroyTransformChildren(m_shoppingList.transform);
        }

        public void Enable()
        {
            Recipe[] recipes = GameState.currentTeyze.recipes;

            for (int i = 0; i < recipes.Length; i++)
            {
                addRecipe(recipes[i]);
            }

            initShoppingList();
            initOwnedIngredientList();
        }

        public void Disable()
        {

        }

        void clearShoppingList()
        {
            List<Transform> tr = new List<Transform>();
            foreach (Transform t in m_recipeIngredients.transform)
            {
                tr.Add(t);
            }

            for (int i = 0; i < tr.Count; i++)
                GameObject.Destroy(tr[i].gameObject);
        }

        void initShoppingList()
        {
            HashSet<string>.Enumerator e = ShoppingListItem.GetAddedItems();


            while (e.MoveNext())
            {
                addToShoppingList(e.Current,false);
            }
        }

        void initOwnedIngredientList()
        {
            IngredientInstance[] instances = GameState.currentTeyze.ingredients;
            for(int i = 0; i<instances.Length;i++)
            {
                addOwnedIngredient(instances[i]);
            }
        }
    }
}
