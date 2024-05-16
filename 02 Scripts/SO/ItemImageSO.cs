using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SO {
    [Serializable]
    public class ItemImage {
        [HideInInspector]
        public string name;
        [SerializeField]
        int _id;
        public int id { 
            get { return this._id; }
            private set { this._id = value; }
        }
        [SerializeField]
        string _description;
        public string description {
            get { return this._description; }
            private set { this._description = value; }
        }
        [SerializeField]
        Sprite _image;
        public Sprite image {
            get { return this._image; }
            private set { this._image = value; }
        }
    }

    [CreateAssetMenu(fileName = "ItemImage", menuName = "SO/ItemImage")]
    public class ItemImageSO : ScriptableObject
    {
        [SerializeField]
        ItemImage[] itemImages;

        #region Unity Method
        void OnValidate() {
            for (int i = 0; i < this.itemImages.Length; i++) {
                var itemImage = this.itemImages[i];
                itemImage.name = $"{itemImage.id} ({itemImage.description})";
            }
        }
        #endregion

        public ItemImage FindById(int id) {
            return Array.Find(this.itemImages, (e) => e.id == id);
        }
    }
}
