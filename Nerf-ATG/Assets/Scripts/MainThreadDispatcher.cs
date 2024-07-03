using System;
using System.Collections.Generic;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher instance;
    private static readonly Queue<Action> executionQueue = new Queue<Action>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        lock (executionQueue)
        {
            while (executionQueue.Count > 0)
            {
                executionQueue.Dequeue().Invoke();
            }
        }
    }

    public static void Execute(Action action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        lock (executionQueue)
        {
            executionQueue.Enqueue(action);
        }
    }

    private void OnDestroy()
    {
        lock (executionQueue)
        {
            executionQueue.Clear();
        }
    }
}

