using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FarrokhGames.Shared;
using TMPro;
using UnityEngine.UI;


namespace FarrokhGames.Inventory.Examples
{
    public class GroundItem : MonoBehaviour
    {
        public ItemDefinition itemDef;
        public string itemName;
        public int durability;
        public string area;
        public string code;
        public string describe;
        public float damage;


        private float rotSpeed = 100f;

        private void Awake()
        {
            itemName = itemDef.itemName; 
            durability = itemDef.durability;
            area = itemDef.area;
            if (area == "") area = "[확인 불가]";
            describe = itemDef.describe;
            damage = itemDef.damage;
        }


        void Update()
        {
            transform.Rotate(new Vector3(0, rotSpeed * Time.deltaTime, 0));
        }

    }
}

