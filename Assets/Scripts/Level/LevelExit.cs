using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace RunningTeyze
{
    public class LevelExit : MonoBehaviour
    {

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Level.SetLevelSuccessful();
            if (collision.tag == "Player") SceneManager.LoadSceneAsync("CookingScene");
           
        }
    }
}
