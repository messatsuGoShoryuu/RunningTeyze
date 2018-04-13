using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RunningTeyze.UI
{ 
    public class ShoppingListUI : MonoBehaviour
    {
        [SerializeField]
        GameObject m_rootGameObject;
        [SerializeField]
        Transform m_contentTransform;
        

        public  void BtnShoppingList()
        {
            m_rootGameObject.SetActive(!m_rootGameObject.activeInHierarchy);
        }


    }

}
