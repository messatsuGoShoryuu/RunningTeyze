using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public struct Ingredient
    {
        public string name;
        public float cost;
        public int rarity;
        public float spPerKg;
    }

    public class IngredientManager
    {
        static List<Ingredient> s_ingredients;
        static Dictionary<string, int> s_ingredientIndices;


        public static void Init()
        {
            s_ingredientIndices = new Dictionary<string, int>();
            s_ingredients = new List<Ingredient>();
            TextAsset text = Resources.Load<TextAsset>("Database/RunningTeyzeIngredients");

            char[] separators = { '\r', '\n', ',' };
            string[] splitted = text.text.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
            Debug.Assert(splitted.Length >= 4);

            for(int i = 4;i<splitted.Length; i+=4)
            {
                Ingredient ing = new Ingredient();

                ing.name = splitted[i];
                float.TryParse(splitted[i + 1], out ing.cost);
                int.TryParse(splitted[i + 2], out ing.rarity);
                float.TryParse(splitted[i + 3], out ing.spPerKg);

                s_ingredientIndices.Add(ing.name, s_ingredients.Count);
                s_ingredients.Add(ing);
            }
        }

        public static Ingredient GetIngredientByName(string name)
        {
            if(s_ingredientIndices.ContainsKey(name))
                return s_ingredients[s_ingredientIndices[name]];

            return new Ingredient();
        }

        public static List<Ingredient> GetRandomIngredientList(int rarity)
        {
            List<Ingredient> result = new List<Ingredient>();
            for(int i = 0; i<s_ingredients.Count;i++)
            {
                if (s_ingredients[i].rarity <= rarity) result.Add(s_ingredients[i]);
            }

            Utilities.Shuffle(result);

            return result;
        }
    }
}
