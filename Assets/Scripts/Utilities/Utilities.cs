using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunningTeyze
{
    public class Utilities
    {
        public static void Shuffle<T>(List<T> list)
        {
            int max = list.Count-1;
            for (int i = 0; i < list.Count; i++)
            {
                T temp = list[max - i];
                int random = Random.Range(0, max - i + 1);
                list[max - i] = list[random];
                list[random] = temp;
            }
        }

        public static GameObject InstantiateFromResources(string path)
        {
            return GameObject.Instantiate<GameObject>
                (Resources.Load<GameObject>(path));
        }

        public static GameObject InstantiateFromResources(string path, Vector2 position)
        {
            return GameObject.Instantiate<GameObject>
                (Resources.Load<GameObject>(path),position,Quaternion.identity);
        }

        public static GameObject InstantiateFromResources(string path, Vector2 position, Quaternion rotation)
        {
            return GameObject.Instantiate<GameObject>
                (Resources.Load<GameObject>(path), position, rotation);
        }

        public static void DestroyTransformChildren(Transform parent)
        {
            List<Transform> tr = new List<Transform>();
            foreach (Transform t in parent.transform)
            {
                tr.Add(t);
            }

            for (int i = 0; i < tr.Count; i++)
                GameObject.Destroy(tr[i].gameObject);
        }
    }
}
