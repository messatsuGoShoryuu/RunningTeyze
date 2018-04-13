using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RunningTeyze.UI
{
    public class ShoppingListItem : MonoBehaviour
    {
        static HashSet<string> s_addedItems;
        static HashSet<string> addedItems
        {
            get
            {
                if (s_addedItems == null) s_addedItems = new HashSet<string>();
                return s_addedItems;
            }
        }

        public static bool Exists(string name) { return addedItems.Contains(name); }

        public static HashSet<string>.Enumerator GetAddedItems()
        {
            return addedItems.GetEnumerator();
        }

        public static void Clear()
        {
            addedItems.Clear();
        }
    
        [SerializeField]
        Text m_text;

        string m_name;
        public void SetInfo(string name)
        {
            if(!addedItems.Contains(name))
                addedItems.Add(name);
            m_text.text = name;
            m_name = name;
        }

        public void BtnDestroy()
        {
            GameObject.Destroy(this.gameObject);
            s_addedItems.Remove(m_name);
        }
    }
}
