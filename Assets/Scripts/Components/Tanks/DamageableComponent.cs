﻿using UnityEngine;

namespace BattleCity.Tanks
{
    [RequireComponent(typeof(BoxCollider))]
    public class DamageableComponent : MonoBehaviour
    {
        [SerializeField] private int _amountOfHealth;

        public Damageable Damageable { get; private set; }
        
        private void Awake()
        {
            Damageable = new Damageable(_amountOfHealth, gameObject);
        }
    }
}
