using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class controls level-related stuff, like success or correct initialization of the player position.

namespace RunningTeyze
{

    public class Level : MonoBehaviour
    {
            static GameObject s_startPosition;
            public static GameObject startPosition
            {
                get
                {
                    if (s_startPosition == null) s_startPosition = GameObject.Find("StartPosition");
                    return s_startPosition;
                }
            }

        static bool s_success = false;
        public static bool success { get { return s_success; } }
        public static void SetLevelSuccessful() { s_success = true; }

        static GameObject s_prefab;
        public static void SetPrefab(GameObject obj)
        {
            s_prefab = obj;
        }


        // Use this for initialization
        void Start()
        {
            if (s_prefab != null) 
                GameObject.Instantiate(s_prefab);
            s_startPosition = GameObject.Find("StartPosition");
            StartCoroutine(CR_LateLoad());
            s_success = false;
        }

        IEnumerator CR_LateLoad()
        {
            yield return new WaitForEndOfFrame();
            LoadCurrentTeyzeCharacter();
        }

        public static void LoadCurrentTeyzeCharacter()
        {
            string basePath = "Prefabs/TeyzeCharacters/";
            GameObject obj = Utilities.InstantiateFromResources(basePath + GameState.currentTeyze.name,startPosition.transform.position);
            CharacterProps props = obj.GetComponent<CharacterProps>();

            EventSystem.Dispatch(new Event_LevelTeyzeLoaded(props));
            CameraFollower.SetTarget(obj.transform);
        }
        
    }
}
