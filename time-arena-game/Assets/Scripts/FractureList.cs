using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FractureList<T>{
    //Array of data items
    T[] data;
    //List of fractures in array
    //Tuple is (position of section in data, origin of fracture, end of fracture)
    List<(int, int, int)> fractures;
    //head always points to next position in array to write to. if head == size then writing is finished.
    int head;
    public FractureList(int size){
        data = new T[size];
        fractures = new List<(int, int, int)>();
        head = 0;
    }

    public int GetSize()
    {
        return data.Length;
    }
    public int GetHead(){
        return head;
    }

    public void AddFracture(int start, int end){
        fractures.Add((head, start, end));
    }

    //appends data at the head and advances head
    public void AddData(T item){
        data[head] = item;
        head++;
    }

    //for appending data later than the head
    public void AddData(T item, int index, bool moveHead = true){
        data[index] = item;
        if(moveHead) head = index;
        head++;
    }
    
    public void RemoveData(int count){
        head -= count;
        fractures.RemoveAll(p => p.Item1 < head);
    }

    public T[] Recall(int index) {
        List<T> output = new List<T>();
        int prevEnd = 0;
        int nextHead = 0;
        foreach ((int, int, int) fracture in fractures) {
            if (index > prevEnd && index < fracture.Item2 && data[nextHead + index] != null) output.Add(data[nextHead + index - prevEnd]);
            prevEnd = fracture.Item3;
            nextHead = fracture.Item1;
        }
        if (nextHead + index - prevEnd > 0 && data[nextHead + index - prevEnd] != null) { output.Add(data[nextHead + index - prevEnd]); }
        return output.ToArray();
    }

    public T DataRecall(int index)
    {
        return data[index];
    }

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