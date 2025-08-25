using UnityEngine;

namespace Resource
{
    public class Food : Resource
    {
        public override ResourceType resourceType { get; protected set; } = ResourceType.Food;
        
        [SerializeField] float _stockPile;
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
            OnStockDepleted += RemoveFood;
            
            _stockPile = Random.Range(10, 15);
        }
        
        private void RemoveFood()
        {
            Destroy(gameObject);
        }
    }
}
