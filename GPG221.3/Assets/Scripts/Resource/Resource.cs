using System;
using System.Numerics;

namespace Resource
{
    public abstract class Resource : TileContent
    {
        public override bool CanWalkOn => false;
        public override Tile Tile { get; protected set; }

        public abstract ResourceType resourceType { get; protected set; }

        public event Action OnStockAdded;
        public event Action OnStockDepleted;
        public abstract float StockPile { get; protected set; }
        
        public static event Action<Tile, ResourceType> OnResourceDepleted;

        protected override void Start()
        {
            base.Start();

            if (Tile)
            {
                transform.position = Tile.transform.position;
            }
        }
        
        public virtual void ConsumeStock(float amount)
        {
            if (amount <= 0)
            {
                print("No amount specified to consume");
            }

            if (StockPile < amount)
            {
                OnStockDepletion();
            }
            StockPile -= amount;
        }
        
        public virtual void AddStock(float amount)
        {
            if (amount <= 0)
            {
                print("No amount specified to add");
            }

            StockPile += amount;
            RaiseOnStockAdded();
        }

        protected virtual void OnStockDepletion()
        {
            OnStockDepleted?.Invoke();
            OnResourceDepleted?.Invoke(Tile, resourceType);
        }

        protected virtual void RaiseOnStockAdded()
        {
            OnStockAdded?.Invoke();
        }
        
    }
}
