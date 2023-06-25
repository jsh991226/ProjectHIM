using UnityEngine;
using System;


namespace FarrokhGames.Inventory.Examples
{
    /// <summary>
    /// Scriptable Object representing an Inventory Item
    /// </summary>
    [CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item", order = 1)]
    public class ItemDefinition : ScriptableObject, IInventoryItem
    {
        [SerializeField] private Sprite _sprite = null;
        [SerializeField] private InventoryShape _shape = null;
        [SerializeField] private ItemType _type = ItemType.Utility; //?? 테스트 해야함
        [SerializeField] private bool _canDrop = true;
        [SerializeField, HideInInspector] private Vector2Int _position = Vector2Int.zero;

        public GameObject itemObject;
        public string itemPrefabName;
        public string spriteName;

        public string itemName;
        public int durability;
        public string area;
        public string code;
        public string describe;
        public float damage;

        private void Awake()
        {
            if (_sprite == null) return;
            this.spriteName = this._sprite.name;
            this.itemPrefabName = this.itemObject.name;
        }

        public IInventoryItem SetItemDefinition(string _spriteName, int _width, int _height, string _canDrop, int _posX, int _posY, string _itemPrefabName, string _dur, string _area, string _code) //플레이어 넣어야함
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("Sprite/" + _spriteName);

            this._sprite = sprites[0];
            this.itemObject = Resources.Load("Prefab/" + _itemPrefabName) as GameObject;
            this._shape = new InventoryShape(_width, _height);
            this._canDrop = Convert.ToBoolean(_canDrop);
            this._position = new Vector2Int(_posX, _posY);
            this.durability = int.Parse(_dur);
            this.area = _area;
            this.code = _code;
            return CreateInstance();
        }


        /// <summary>
        /// The name of the item
        /// </summary>
        public string Name => this.name;

        /// <summary>
        /// The type of the item
        /// </summary>
        public ItemType Type => _type;

        /// <inheritdoc />
        public Sprite sprite => _sprite;

        /// <inheritdoc />
        public int width => _shape.width;

        /// <inheritdoc />
        public int height => _shape.height;

        /// <inheritdoc />
        public Vector2Int position
        {
            get => _position;
            set => _position = value;
        }

        /// <inheritdoc />
        public bool IsPartOfShape(Vector2Int localPosition)
        {
            return _shape.IsPartOfShape(localPosition);
        }

        /// <inheritdoc />
        public bool canDrop => _canDrop;
         
        string IInventoryItem.code => code; 

        /// <summary>
        /// Creates a copy if this scriptable object
        /// </summary>
        public IInventoryItem CreateInstance()
        {
            var clone = ScriptableObject.Instantiate(this);
            clone.itemName = this.itemName;
            clone.durability = this.durability;
            clone.area = this.area;
            clone.code = this.code;
            clone.describe = this.describe;
            clone.damage = this.damage;
            clone.name = clone.name.Substring(0, clone.name.Length - 7); // Remove (Clone) from name
            return clone; 
        }
    }
}