using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public  struct TeyzeProps
    {
        public string name;
        public float wealth;
        public float salary;
        public DamageModifier damageModifier;
        public float resistance;
        public float weight;
        public float speed;
        public float strength;
        public float maxHealth;
        public List<string> recipes;
        public float reputation;
        public float cooking;
    }

    public class TeyzeManager
    {
        public static Dictionary<string, TeyzeProps> s_props;
        public static Dictionary<string, Teyze> s_teyzeInstances;

        public static void Init()
        {
            s_teyzeInstances = new Dictionary<string, Teyze>();
            s_props = new Dictionary<string, TeyzeProps>();

            TextAsset text = Resources.Load<TextAsset>("Database/RunningTeyzeTeyzeList");

            char[] separators = { '\r', '\n'};
            string[] lines = text.text.Split(separators,System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < lines.Length; i+=2)
            {
                char[] comma = { ',' };
                string[] character = lines[i].Split(comma, System.StringSplitOptions.RemoveEmptyEntries);
                string[] recipes = lines[i+1].Split(comma, System.StringSplitOptions.RemoveEmptyEntries);

                TeyzeProps props = new TeyzeProps();
                props.name = character[0];

                DamageModifier mod = new DamageModifier();
                float.TryParse(character[1], out props.wealth);
                float.TryParse(character[2], out props.salary);
                float.TryParse(character[3], out mod.damageModifier);
                float.TryParse(character[4], out props.resistance);
                float.TryParse(character[5], out props.weight);
                float.TryParse(character[6], out props.speed);
                float.TryParse(character[7], out props.strength);
                float.TryParse(character[8], out props.maxHealth);
                float.TryParse(character[9], out props.reputation);
                float.TryParse(character[10], out props.cooking);

                mod.celerityModifier = 1.0f;
                props.damageModifier = mod;

                props.recipes = new List<string>();
                for(int j = 0; j<recipes.Length;j++)
                {
                    props.recipes.Add(recipes[j]);
                }
                s_props.Add(props.name, props);
            }

            foreach(KeyValuePair<string, TeyzeProps> p in s_props)
            {
                s_teyzeInstances.Add(p.Value.name, new Teyze(p.Value.name));
            }
        }

        public static TeyzeProps GetProps(string name)
        {
            return s_props[name];
        }

        public static Teyze GetInstance(string name)
        {
            return s_teyzeInstances[name];
        }
    }

    public struct IngredientInstance
    {
        public  Ingredient ingredient;
        public float amountKg;
    }

    public class Teyze
    {
        string m_name;
        public string name { get { return m_name; } }

        float m_wealth;
        public float wealth { get { return m_wealth; } }

        float m_salary;
        public float salary { get { return m_salary; } }

        DamageModifier m_damageModifier;
        public DamageModifier damageModifier { get { return m_damageModifier; } }

        float m_resistance;
        public  float resistance { get { return m_resistance; } }

        float m_weight;
        public float weight { get { return m_weight; } }

        float m_speed;
        public float speed { get { return m_speed; } }

        float m_strength;
        public float strength { get { return m_strength; } }

        float m_maxHealth;
        public float maxHealth { get { return m_maxHealth; } }

        float m_skillPointsToDistribute = 0;
        public float skillPointsToDistribute { get { return m_skillPointsToDistribute; } }

        List<Recipe> m_recipes;
        public Recipe[] recipes { get { return m_recipes.ToArray(); } }

        List<IngredientInstance> m_ingredients;
        public IngredientInstance[] ingredients { get{ return m_ingredients.ToArray(); } }

        Dictionary<Teyze,float> m_standings;

        float m_reputation;
        public float reputation { get { return m_reputation; } }

        float m_cooking;
        public float cooking { get { return m_cooking; } }

        public Teyze(string name)
        {
            m_recipes = new List<Recipe>();
            m_standings = new Dictionary<Teyze, float>();
            m_ingredients = new List<IngredientInstance>();

            TeyzeProps props = TeyzeManager.GetProps(name);
            for(int i = 0; i<props.recipes.Count;i++)
            {
                m_recipes.Add(RecipeManager.GetRecipeByName(props.recipes[i]));
            }

            m_maxHealth = props.maxHealth;
            m_damageModifier = props.damageModifier;
            m_cooking = props.cooking;
            m_reputation = props.reputation;
            m_resistance = props.resistance;
            m_name = props.name;
            m_wealth = props.wealth;
            m_salary = props.salary;
            m_weight = props.weight;
            m_strength = props.strength;
            m_speed = props.speed;
        }

        public static bool Cook(Recipe recipe)
        {
            
            if (GameState.currentTeyze == null) return false;

            if (!GameState.currentTeyze.HasRecipe(recipe)) return false;

            List<int> foundIngredients = new List<int>(16);

            for(int i = 0; i<recipe.ingredients.Length;i++)
            {
                int index = GameState.currentTeyze.FindIngredientIndex(recipe.ingredients[i].ingredient);
                if (index < 0) return false;
                if (GameState.currentTeyze.ingredients[index].amountKg < recipe.ingredients[i].amountKg) return false;
                foundIngredients.Add(index);
            }

            for(int i = 0; i<foundIngredients.Count;i++)
            {
                GameState.currentTeyze.ingredients[i].amountKg -= recipe.ingredients[i].amountKg;
            }

            GameState.currentTeyze.m_skillPointsToDistribute += recipe.yieldSP; 

            EventSystem.Dispatch(new Event_RecipeCooked(recipe));
            return true;
        }

        public bool HasRecipe(Recipe recipe)
        {
            for(int i = 0; i<m_recipes.Count;i++)
            {
                if (m_recipes[i].name == recipe.name) return true;
            }
            return false;
        }

        public int FindIngredientIndex(Ingredient ingredient)
        {
            for(int i = 0; i<m_ingredients.Count; i++)
            {
                if (m_ingredients[i].ingredient.name == ingredient.name) return i;
            }
            return -1;
        }

        public bool BuyIngredient(float cost, Ingredient ingredient, float amount)
        {        
            if (m_wealth < cost) return false;
            m_wealth -= cost;

            int index = -1;
            for(int i = 0; i<m_ingredients.Count;i++)
            {
               if(m_ingredients[i].ingredient.name == ingredient.name)
                {
                    index = i;
                    break;
                }
            }

            if(index == -1)
            {
                IngredientInstance instance = new IngredientInstance();
                instance.ingredient = ingredient;
                instance.amountKg = amount;
                m_ingredients.Add(instance);
            }
            else
            {
                IngredientInstance temp = m_ingredients[index];
                temp.ingredient = ingredient;
                temp.amountKg += amount;
                m_ingredients[index] = temp;
            }
            
            
            return true;
        }


        public void CHEAT_addRequiredIngredients()
        {
            for(int i = 0; i<m_recipes.Count;i++)
            {
                for(int j = 0; j<m_recipes[i].ingredients.Length;j++)
                {
                    BuyIngredient(0.0f, m_recipes[i].ingredients[j].ingredient, 10.0f);
                }
            }

            Debug.Log("CHEAT: Bought 10 kg of each ingredient necessary per recipe");
        }

    }
}
