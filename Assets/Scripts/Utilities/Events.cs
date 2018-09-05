using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public class Event_PazarRequested : BaseEventData
    {
        public Event_PazarRequested(GameObject obj, bool isPlayer)
        {
            this.obj = obj;
            this.isPlayer = isPlayer;
        }
        public GameObject obj;
        public bool isPlayer;
    }

    public class Event_PazarShowUI : BaseEventData
    {
        public Event_PazarShowUI(Pazar pazar)
        {
            this.pazar = pazar;
        }
        public Pazar pazar;
    }

    public class Event_PlayerIngredientBuyRequest : BaseEventData
    {
        public Event_PlayerIngredientBuyRequest(string name, float amount)
        {
            this.name = name;
            this.amount = amount;
        }
        public string name;
        public float amount;
    }

    public class Event_PlayerIngredientBought : BaseEventData
    {
        public Event_PlayerIngredientBought(Ingredient ingredient, float kg)
        {
            this.ingredient = ingredient;
            this.kg = kg;
        }
        public Ingredient ingredient;
        public float kg;
    }

    public class Event_PlayerStateChanged : BaseEventData { }

    public class Event_RecipeUIOnRecipeHovered : BaseEventData
    {
        public Event_RecipeUIOnRecipeHovered(Recipe recipe)
        {
            this.recipe = recipe;
        }
        public Recipe recipe;
    }

    public class Event_AddToShoppingList : BaseEventData
    {
        public Event_AddToShoppingList(string name)
        {
            this.name = name;
        }

        public string name;
    }

    public class Event_RecipeCooked : BaseEventData
    {
        public Event_RecipeCooked(Recipe recipe)
        {
            this.recipe = recipe;
        }
        public Recipe recipe;
    }

    public class Event_LevelTeyzeLoaded : BaseEventData
    {
        public Event_LevelTeyzeLoaded(CharacterProps props)
        {
            this.props = props;
        }

        public CharacterProps props;
    }

    public class Event_CurrentTeyzeChanged : BaseEventData
    {
        public Event_CurrentTeyzeChanged(Teyze newTeyze, Teyze oldTeyze)
        {
            this.newTeyze = newTeyze;
            this.oldTeyze = oldTeyze;
        }

        public Teyze newTeyze;
        public Teyze oldTeyze;
    }

    public class Event_DistrictNeighborhoodSelected : BaseEventData
    {
        public Event_DistrictNeighborhoodSelected(DistrictNeighborhood neighborhood)
        {
            this.neighborhood = neighborhood;
        }

        public DistrictNeighborhood neighborhood;

    }

    public class Event_DistrictNavigationInitialized : BaseEventData
    {
        public Event_DistrictNavigationInitialized(DistrictNavigationItem[] items)
        {
            this.items = items;
        }

        public DistrictNavigationItem[] items;
    }

    public class Event_DistrictNeighborhoodUnlocked : BaseEventData
    {
        public Event_DistrictNeighborhoodUnlocked(DistrictNeighborhood neighborhood)
        {
            this.neighborhood = neighborhood;
        }

        public DistrictNeighborhood neighborhood;
    }

    public class Event_DistrictNeighborhoodCompleted : BaseEventData
    {
        public Event_DistrictNeighborhoodCompleted(DistrictNeighborhood neighborhood)
        {
            this.neighborhood = neighborhood;
        }

        public DistrictNeighborhood neighborhood;
    }

}