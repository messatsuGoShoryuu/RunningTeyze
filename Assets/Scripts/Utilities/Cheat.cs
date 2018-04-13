using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public class Cheat : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.U))
            {
                if(GameState.currentTeyze != null)
                {
                    GameState.currentTeyze.CHEAT_addRequiredIngredients();
                }
                    
            }
        }
    }
}
