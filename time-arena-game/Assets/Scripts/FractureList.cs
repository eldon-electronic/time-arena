using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FractureList<T>{
    T[] data;
    List<(int, int, int)> fractures;
    int head;
    public FractureList(int size){
        data = new T[size];
        fractures = new List<(int, int, int)>();
        fractures.Add((0,0,0));
        head = 0;
    }

    public void addFracture(int start, int end){
        fractures.Add((head, start, end));
    }

    public void addData(T item){
        data[head] = item;
        head++;
    }

    public void removeData(int count){
        head -= count;
        fractures.RemoveAll(p => p.Item1 < head);
    }

    public T[] recall(int index){
        List<T> output = new List<T>();
        int prevEnd = 0;
        int nextHead = 0;
        foreach((int, int, int) fracture in fractures.Skip(1)){
            if(index > prevEnd && index < fracture.Item2) output.Add(data[nextHead + index]);
            prevEnd = fracture.Item3;
            nextHead = fracture.Item1;
        }
        if(nextHead + index < data.Length) output.Add(data[nextHead + index]);
        return output.ToArray();
    }
}