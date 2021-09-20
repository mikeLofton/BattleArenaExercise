using System;
using System.Collections.Generic;
using System.Text;

namespace BattleArena
{
    class Entity
    {
        private string _name;
        private float _health;
        private float _attackPower;
        private float _defensePower;

        public string Name
        {
            get { return _name; }
        }

        public float Health
        {
            get { return _health; }
        }

        public virtual float AttackPower
        {
            get { return _attackPower; }                                    
        }

        public virtual float DefensePower
        {
            get { return _defensePower; }
        }

        public Entity()
        {
            _name = "Default";
            _health = 0;
            _attackPower = 0;
            _defensePower = 0;
        }

        public Entity(string name, float health, float attackPower, float defensePower)
        {
            _name = name;
            _health = health;
            _attackPower = attackPower;
            _defensePower = defensePower;
        }

        public float TakeDamage(float damageAmount)
        {
            float damageTaken = damageAmount - DefensePower;

            if (damageTaken < 0)
            {
                damageTaken = 0;
            }

            _health -= damageTaken;

            return damageTaken;
        }

        public float Attack(Entity defender)
        {
            return defender.TakeDamage(AttackPower);
        }
    }
}
