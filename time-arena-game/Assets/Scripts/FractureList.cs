using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FractureList<T>
{
    // Array of data items.
    private T[] _data;

    // List of fractures in array.
    // Tuple is (position of section in data, origin of fracture, end of fracture).
    private List<(int, int, int)> _fractures;

    // _head always points to next position in array to write to. If head == size then writing is finished.
    private int _head;

    public FractureList(int size)
    {
        _data = new T[size];
        _fractures = new List<(int, int, int)>();
        _head = 0;
    }

    public int GetSize() { return _data.Length; }

    public int GetHead() { return _head; }

    public void AddFracture(int start, int end) { _fractures.Add((_head, start, end)); }

    // Appends data at the head and advances head.
    public void AddData(T item)
    {
        _data[_head] = item;
        _head++;
    }

    // For appending data later than the head.
    public void AddData(T item, int index, bool moveHead = true)
    {
        _data[index] = item;
        if (moveHead) _head = index;
        _head++;
    }
    
    public void RemoveData(int count)
    {
        _head -= count;
        _fractures.RemoveAll(p => p.Item1 < _head);
    }

    public T[] Recall(int index)
    {
        List<T> output = new List<T>();
        int prevEnd = 0;
        int nextHead = 0;
        foreach ((int, int, int) fracture in _fractures)
        {
            if (index > prevEnd && index < fracture.Item2 && _data[nextHead + index] != null)
            {
                output.Add(_data[nextHead + index - prevEnd]);
            }
            prevEnd = fracture.Item3;
            nextHead = fracture.Item1;
        }
        if (nextHead + index - prevEnd > 0 && _data[nextHead + index - prevEnd] != null)
        {
            output.Add(_data[nextHead + index - prevEnd]);
        }
        return output.ToArray();
    }

    public T DataRecall(int index) { return _data[index]; }

    public static string Test()
    {
        FractureList<int> testList = new FractureList<int>(15);
        testList.AddData(testList.GetHead());
        testList.AddData(testList.GetHead());
        testList.AddData(testList.GetHead());
        testList.AddData(testList.GetHead());
        testList.AddData(testList.GetHead());
        testList.AddFracture(5, 2);
        testList.AddData(testList.GetHead());
        testList.AddData(testList.GetHead());
        testList.AddData(testList.GetHead());
        testList.AddData(testList.GetHead());
        testList.AddFracture(6, 3);
        testList.AddData(testList.GetHead());
        testList.AddData(testList.GetHead());
        testList.AddData(testList.GetHead());
        int[] o = testList.Recall(4);
        return string.Join(" ", o);
    }
}
