using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T> where T: IComparable<T>
{
    public List<T> _heap = new List<T>();

    public int Count => _heap.Count;
    
    public T Contains(T t)
    {
        int idx = _heap.IndexOf(t);
        if (idx < 0) return default(T);
        return _heap[idx];
    }

    public void Push(T data)
    {
        _heap.Add(data); //리스트의 맨 끝에 넣는다
        int now = _heap.Count - 1; //맨 마지막 원소의 인덱스를 알아내서 now에 저장한다 

        while (now > 0) //꼭대기에 있지 않다. 근데 내 자리 찾으면 브레이크
        {
            int next = (now - 1) / 2; //위에 누가 있는가
            if (_heap[now].CompareTo(_heap[next]) < 0)
                break;
            
            T temp = _heap[now];
            _heap[now] = _heap[next];
            _heap[next] = temp;

            now = next;
        }
    }

    public T Pop()
    {
        T ret = _heap[0];

        int lastIndex = _heap.Count - 1;
        _heap[0] = _heap[lastIndex];
        _heap.RemoveAt(lastIndex); //참고로 마지막을 없애는 건 리스트 복사를 안일으킨다
        lastIndex--;

        int now = 0;

        while (true)
        {
            int left = 2 * now + 1; //왼쪽 자손 인덱스
            int right = 2 * now + 2; //오른쪽 자손 인덱스

            int next = now;
            //왼쪽이냐 오른쪽이냐 검사를 할껀데

            if (left <= lastIndex && _heap[next].CompareTo(_heap[left]) < 0)
                next = left;

            if (right <= lastIndex && _heap[next].CompareTo(_heap[right]) < 0)
                next = right;

            //검사를 끝내고
            if (next == now)
                break; //내 자리를 찾았다

            //변경이 일어났다면 여기서 next와 now를 교체하는 로직을 작성하면 된다 
            T temp = _heap[now];
            _heap[now] = _heap[next];
            _heap[next] = temp;

            now = next;
        }

        return ret;
    }

    //Peek Pop
    public T Peek()
    {
        return _heap.Count == 0 ? default(T) : _heap[0];
    }

    public void Clear()
    {
        _heap.Clear(); //힙 지우기
    }
}
