using System;

public class MainThreadExecutor : IMainThreadExecutor
{
    public void Execute(Action action)
    {
        MainThreadDispatcher.Execute(action);
    }
}