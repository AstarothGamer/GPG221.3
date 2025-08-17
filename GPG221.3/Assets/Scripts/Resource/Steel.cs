using UnityEngine;

namespace Resource
{
    public class Steel : Resource
    {
        public override ResourceType resourceType { get; protected set; } = ResourceType.Steel;
        
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
            OnStockDepleted += RemoveWater;
            
            _stockPile = Mathf.Infinity;
        }
        
        public override void ConsumeStock(float amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning("No amount specified to consume");
                return;
            }

            Debug.Log("Consumed water");
        }
        private void RemoveWater()
        {
            Destroy(gameObject);
        }
    }
}
