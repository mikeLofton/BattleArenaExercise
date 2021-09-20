using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BattleArena
{
    class Player : Entity
    {
        private Item[] _items;
        private Item _currentItem;
        private int _currentItemIndex;
        private string _job;

        public override float DefensePower
        {
            get
            {
                if (_currentItem.Type == ItemType.DEFENSE)
                    return base.DefensePower + CurrentItem.StatBoost;

                return base.DefensePower;
            }
        }

        public override float AttackPower
        { 
            get
            {
                if (_currentItem.Type == ItemType.ATTACK)
                    return base.AttackPower + CurrentItem.StatBoost;

                return base.AttackPower;                
            }
        }

        public Item CurrentItem
        {
            get { return _currentItem; }
        }

        public string Job
        {
            get 
            { 
                return _job;
            }

            set
            { 
                _job = value; 
            }
        }

        public Player()
        {
            _items = new Item[0];
            _currentItem.Name = "Nothing";
            _currentItemIndex = -1;
        }

        public Player(Item[] items) : base()
        {
            _currentItem.Name = "Nothing";
            _items = items;
            _currentItemIndex = -1;
        }

        public Player(string name, float health, float attackPower, float defensePower, Item[] items, string job) : base(name, health, attackPower, defensePower)
        {
            _items = items;
            _currentItem.Name = "Nothing";
            _job = job;
        }

        /// <summary>
        /// Sets the item at the given index to be the current item
        /// </summary>
        /// <param name="index">The index of the item in the array</param>
        /// <returns>False if the index is outside the bounds of the array</returns>
        public bool TryEquipItem(int index)
        {
            //If the index is out of bounds...
            if (index >= _items.Length || index < 0)
            {
                //...return false
                return false;
            }

            _currentItemIndex = index;

            //Set the current item to be the array at the given index
            _currentItem = _items[_currentItemIndex];

            return true;
        }

        /// <summary>
        /// Set the current item to be nothing
        /// </summary>
        /// <returns>False if there is no item equipped</returns>
        public bool TryRemoveCurrentItem()
        {
            //If the current item is set to nothing...
            if (CurrentItem.Name == "Nothing")
            {
                //...return false
                return false;
            }

            _currentItemIndex = -1;

            //Set item to be nothing
            _currentItem = new Item();
            _currentItem.Name = "Nothing";

            return true;
        }
       
        /// <returns>Gets the item names</returns>
        public string[] GetItemNames()
        {
            string[] itemNames = new string[_items.Length];

            for (int i = 0; i < _items.Length; i++)
            {
                itemNames[i] = _items[i].Name;
            }

            return itemNames;
        }

        public override void Save(StreamWriter writer)
        {
            writer.WriteLine(_job);
            base.Save(writer);
            writer.WriteLine(_currentItemIndex);        
        }

        public override bool Load(StreamReader reader)
        {    
            //If the base loading function fails...
            if (!base.Load(reader))
                //...return false
                return false;

            //If the current line can't be converted into an int...
            if (!int.TryParse(reader.ReadLine(), out _currentItemIndex))
                //...return false
                return false;

            //Return whether or not the item was equipped successfully
           return TryEquipItem(_currentItemIndex);

            
            
        }
    }
}
