using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class InventoryDoorController : MonoBehaviour
    {
        public List<DoorData> doorData;

        #region Unity Functions

        private void Start()
        {
            bool[] collectList = InventorySystem.Instance.GetCollectionList();
            for (var i = 0; i < collectList.Length; i++)
            {
                bool collect = collectList[i];
                if (collect)
                {
                    foreach (GameObject lighting in doorData[i].doorLighting)
                    {
                        lighting.SetActive(false);
                    }
                }
            }
        }

        #endregion

        #region Struct

        [System.Serializable]
        public struct DoorData
        {
            public List<GameObject> doorLighting;
        }

        #endregion
    }
}