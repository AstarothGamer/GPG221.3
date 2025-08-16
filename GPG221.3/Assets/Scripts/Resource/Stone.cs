using UnityEngine;

namespace Resource
{
    public class Stone :Resource
    {
        public override ResourceType resourceType { get; protected set; } = ResourceType.Stone;
        
        private float _stockPile;
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
            OnStockDepleted += RemoveStone;
            
            _stockPile = Random.Range(15,30);
        }
        
        private void RemoveStone()
        {
            Destroy(gameObject);
        }
    }
}
