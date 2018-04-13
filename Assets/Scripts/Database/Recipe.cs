using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public class Recipe
    {
        public string name;
        public float difficultySP;
        public float yieldSP;
        public int rarity;
        public IngredientInstance[] ingredients;
    }
    public  class  RecipeManager
    {
        static List<Recipe> s_recipes;
        static Dictionary<string, int> s_recipeHandles;

        public static void Init()
        {
            s_recipes = new List<Recipe>();
            s_recipeHandles = new Dictionary<string, int>();

            TextAsset text = Resources.Load<TextAsset>("Database/RunningTeyzeRecipies");

            char[] separators = { '\r', '\n', ',' };
            string[] splitted = text.text.Split(separators, System.StringSplitOptions.None);

            Recipe r = null;
            float sp = 0.0f;
            List<IngredientInstance> ingredients = new List<IngredientInstance>();
            for(int i = 6; i<splitted.Length;i+=6)
            {
                
                if (!string.IsNullOrEmpty(splitted[i]))
                {

                    if(r!=null)
                    {
                        r.ingredients = ingredients.ToArray();
                        r.yieldSP = sp;
                    }

                    ingredients.Clear();
                    r = new Recipe();
                    r.name = splitted[i];
                    s_recipes.Add(r);
                    s_recipeHandles.Add(r.name, s_recipes.Count - 1);

                    sp = 0.0f;
                }
                IngredientInstance p = new IngredientInstance();
                p.ingredient = IngredientManager.GetIngredientByName(splitted[i+3]);
                float.TryParse(splitted[i+4], out p.amountKg);

                ingredients.Add(p);
                sp += p.ingredient.spPerKg * p.amountKg;
            }

            r.ingredients = ingredients.ToArray();
            r.yieldSP = sp;
        }

        public static List<Recipe> GetRandomRecipe(int rarity)
        {
            List<Recipe> result = new List<Recipe>();
            for (int i = 0; i < s_recipes.Count; i++)
            {
                if (s_recipes[i].rarity <= rarity) result.Add(s_recipes[i]);
            }

            Utilities.Shuffle(result);

            return result;
        }

        public static Recipe GetRecipeByName(string name)
        {
            return s_recipes[s_recipeHandles[name]];
        }
    }
}
