using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Resource
{
    public class Wood : Resource
    {
        [SerializeField] public override ResourceType resourceType { get; protected set; } = ResourceType.Wood;

        [SerializeField] private float _stockPile;
        public override float StockPile
        {
            get => _stockPile;
            protected set
            {
                _stockPile = value;
                if (_stockPile <= 0) // fallback(?) for stock depletion
                {
                    OnStockDepletion();
                }
            }
        }
        
        protected override void Start()
        {
            base.Start();
            
            OnStockDepleted += RemoveTree;
            
            _stockPile = Random.Range(6,18);
        }
        
        private void RemoveTree()
        {
            Destroy(gameObject);
        }
    }
}