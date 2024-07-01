using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class TaskQueue
{
    private readonly SortedList<int, Queue<Action>> _tasks = new SortedList<int, Queue<Action>>();
    private readonly AutoResetEvent _taskAvailable = new AutoResetEvent(false);
    private readonly object _lock = new object();

    public void EnqueueTask(Action task, int priority)
    {
        lock (_lock)
        {
            if (!_tasks.TryGetValue(priority, out var queue))
            {
                queue = new Queue<Action>();
                _tasks[priority] = queue;
            }
            queue.Enqueue(task);
        }
        _taskAvailable.Set();
    }

    public Action DequeueTask()
    {
        _taskAvailable.WaitOne();
        lock (_lock)
        {
            if (_tasks.Any())
            {
                var highestPriority = _tasks.Keys.Min();
                var queue = _tasks[highestPriority];
                var task = queue.Dequeue();
                if (queue.Count == 0)
                {
                    _tasks.Remove(highestPriority);
                }
                return task;
            }
        }
        return null; // Or handle this scenario differently
    }

    public bool TryDequeueTask(out Action task)
    {
        lock (_lock)
        {
            if (_tasks.Any())
            {
                var highestPriority = _tasks.Keys.Min();
                var queue = _tasks[highestPriority];
                task = queue.Dequeue();
                if (queue.Count == 0)
                {
                    _tasks.Remove(highestPriority);
                }
                return true;
            }
        }
        task = null;
        return false;
    }

    public bool IsEmpty
    {
        get
        {
            lock (_lock)
            {
                return !_tasks.Any();
            }
        }
    }
}