using System;
using System.Linq;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int currentItemCount;
    public int Count => currentItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public bool Any(Func<T, bool> check)
    {
        return items.Any(x => check(x));
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
        SortDown(item);
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }


    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    void SortDown(T item)
    {
        int childIndexLeft = item.HeapIndex * 2 + 1;
        int childIndexRight = item.HeapIndex * 2 + 2;
        int swapIndex = 0;

        if (childIndexLeft < currentItemCount)
        {
            swapIndex = childIndexLeft;

            if (childIndexRight < currentItemCount
                && items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                swapIndex = childIndexRight;

            if (item.CompareTo(items[swapIndex]) < 0)
            {
                Swap(item, items[swapIndex]);
                SortDown(item);
            }
        }
    }

    void SortUp(T item)
    {
        T parentItem = items[(item.HeapIndex - 1) / 2];
        if (item.CompareTo(parentItem) > 0)
        {
            Swap(item, parentItem);
            SortUp(item);
        }
    }

    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = items[itemB.HeapIndex];
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }

}
