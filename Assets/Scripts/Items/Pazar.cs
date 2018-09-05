using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The Pazar is the shop of the game. This is where we buy our ingredients.
namespace RunningTeyze
{
    public class Pazar : MonoBehaviour
    {
        //Defines the rarity level of the items. A Pazar with a rarity of 0 will only sell items of rarity 0, but a Pazar with a rarity of 3 will sell items of rarity 0, 1, 2 and 3.
        [SerializeField]
        int m_rarity = 0;

        //How cheap does this pazar sell its stuff?
        [SerializeField]
        float m_costMultiplier = 1.0f;
        public float costMultiplier { get { return m_costMultiplier; } }

        [SerializeField]
        float m_kgBase = 10.0f;
        [SerializeField]
        float m_quantityVariance = 2.0f;

        [SerializeField]
        int m_maxIngredientType = 10;

        List<Ingredient> m_ingredients;
        public List<Ingredient> ingredients { get { return m_ingredients; } }

        List<GameObject> m_clientList;

        // Use this for initialization
        void Start()
        {
            m_ingredients = new List<Ingredient>();
            m_clientList = new List<GameObject>();
            GenerateIngredientList();
            EventSystem.AddListener<Event_PazarRequested>(OnPazarRequest);
            EventSystem.AddListener<Event_PlayerIngredientBuyRequest>(OnPlayerBuy);
        }

        private void OnDestroy()
        {
            EventSystem.RemoveListener<Event_PazarRequested>(OnPazarRequest);
            EventSystem.RemoveListener<Event_PlayerIngredientBuyRequest>(OnPlayerBuy);
        }
        void GenerateIngredientList()
        {
            m_ingredients.Clear();
            List<Ingredient> fullList = IngredientManager.GetRandomIngredientList(m_rarity);

            int maxCount = Mathf.Min(m_maxIngredientType, fullList.Count);

            for (int i = 0; i < maxCount; i++)
            {
                m_ingredients.Add(fullList[i]);
            }

        }

        void OnPazarRequest(Event_PazarRequested e)
        {
            if (m_clientList.Contains(e.obj))
            {
                if (e.isPlayer)
                    EventSystem.Dispatch(new Event_PazarShowUI(this));
            }
        }

        void OnPlayerBuy(Event_PlayerIngredientBuyRequest e)
        {
            Ingredient ingredient = findIngredientByName(e.name);
            bool purchaseSucceeded = GameState.currentTeyze.BuyIngredient(ingredient.cost * m_costMultiplier * e.amount, ingredient, e.amount);

            if (purchaseSucceeded)
            {
                EventSystem.Dispatch(new Event_PlayerIngredientBought(ingredient, e.amount));
                EventSystem.Dispatch(new Event_PlayerStateChanged());
            }
        }

        Ingredient findIngredientByName(string name)
        {
            for (int i = 0; i < m_ingredients.Count; i++)
                if (m_ingredients[i].name == name)return m_ingredients[i];
            return m_ingredients[0];
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            m_clientList.Add(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            m_clientList.Remove(other.gameObject);
        }
    }
}