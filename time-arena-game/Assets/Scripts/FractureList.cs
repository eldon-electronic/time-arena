using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FractureList<T>{
    //Array of data items
    List<(int, T)> data;
    int nextOffset = 0;
    public FractureList(int size){
        data = new List<(int, T)>();
    }

    public int GetSize()
    {
        return data.Count();
    }

    public void AddFracture(int start, int end){
        nextOffset = end - start;
    }

    //appends data at the head and advances head
    public void AddData(T item){
        if(data.Count == 0){
            data.Add((0, item));
            return;
        }
        data.Add((data[data.Count - 1].Item1 + 1 + nextOffset, item));
        nextOffset = 0;
    }

    //for appending data later than the head
    /*public void AddData(T item, int index, bool moveHead = true){
        data[index] = item;
        if(moveHead) head = index;
        head++;
    }*/
    
    public void RemoveData(int count){
        data.RemoveRange(data.Count - 1 - count, data.Count - 1);
    }

    public T[] Recall(int index) {
        List<T> output = new List<T>();
        foreach ((int, T) d in data) {
            if (d.Item1 == index) output.Add(d.Item2);
        }
        return output.ToArray();
    }

    public T DataRecall(int index)
    {
        return data[index].Item2;
    }
}