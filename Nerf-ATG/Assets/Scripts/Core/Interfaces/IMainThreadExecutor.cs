using System;

public interface IMainThreadExecutor
{
    void Execute(Action action);
}
