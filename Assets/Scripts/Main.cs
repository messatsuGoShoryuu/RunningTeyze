
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace RunningTeyze
{
    public class Main : MonoBehaviour
    {
        [SerializeField]
        string m_nextScene;
        // Use this for initialization
        void Awake()
        {
            IngredientManager.Init();
            RecipeManager.Init();
            TeyzeManager.Init();
            GameState.SetCurrentTeyze("Nermin");
            SceneManager.LoadSceneAsync(m_nextScene);
        }

    }
}
