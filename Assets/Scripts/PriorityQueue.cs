using System;
using System.Collections.Generic;

public class PriorityQueue<TElement, TPriority>
{
    private List<(TElement, TPriority)> heap = new List<(TElement, TPriority)>();
    private readonly IComparer<TPriority> comparer = Comparer<TPriority>.Default;

    public int Count => heap.Count;

    public void Enqueue(TElement element, TPriority priority)
    {
        heap.Add((element, priority));

        int i = heap.Count - 1;
        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (comparer.Compare(heap[i].Item2, heap[parent].Item2) >= 0)
            {
                break;
            }

            Swap(i, parent);
            i = parent;
        }
    }

    public TElement Dequeue()
    {
        if (heap.Count <= 0)
        {
            throw new InvalidOperationException("Queue가 비었습니다.");
        }

        var root = heap[0].Item1;
        heap[0] = heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);

        int i = 0;
        while (true)
        {
            int left = 2 * i + 1;
            int right = 2 * i + 2;
            int smallest = i;

            if (left < heap.Count && comparer.Compare(heap[left].Item2, heap[smallest].Item2) < 0)
            {
                smallest = left;
            }

            if (right < heap.Count && comparer.Compare(heap[right].Item2, heap[smallest].Item2) < 0)
            {
                smallest = right;
            }

            if (smallest == i)
            {
                break;
            }

            Swap(i, smallest);
            i = smallest;
        }

        return root;
    }

    public TElement Peek()
    {
        if (heap.Count <= 0)
        {
            throw new InvalidOperationException("Queue가 비었습니다.");
        }

        return heap[0].Item1;
    }

    private void Swap(int a, int b)
    {
        var temp = heap[a];
        heap[a] = heap[b];
        heap[b] = temp;
    }

    public void Clear()
    {
        heap.Clear();
    }
}