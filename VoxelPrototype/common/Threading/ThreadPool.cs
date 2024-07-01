using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelPrototype.common.Threading
{
    using System;
    using System.Threading;

    public class VoxelPrototypeThreadPool
    {
        private readonly List<Thread> _workers;
        private readonly TaskQueue _taskQueue;
        string Name;
        int Counter;
        int WorkerCount;
        public VoxelPrototypeThreadPool(int workerCount,string name, TaskQueue taskQueue)
        {
            WorkerCount = workerCount;
            this.Name = name;
            _workers = new List<Thread>(workerCount);
            _taskQueue = taskQueue;

            for (int i = 0; i < workerCount; i++)
            {
                Counter++;
                _workers[i] = new Thread(Work);
                _workers[i].Name = $"{Name} Worker Thread no {Counter}";
                _workers[i].IsBackground = true;
                _workers[i].Start();
            }
        }
        public void SetWorkerThreadNumber(int workerCount)
        {
            if(workerCount > WorkerCount)
            {
                for(int i = 0;i <workerCount - WorkerCount;i++)
                {
                    Counter++;
                    _workers[i] = new Thread(Work);
                    _workers[i].Name = $"{Name} Worker Thread no {Counter}";
                    _workers[i].IsBackground = true;
                    _workers[i].Start();
                }
            }else
            {
                for (int i = 0; i < WorkerCount - workerCount ; i++)
                {
                    _workers.RemoveAt(Counter - 1);
                    Counter--;
                }
            }
            WorkerCount= workerCount;
        }
        private void Work()
        {
            while (true)
            {
                var task = _taskQueue.DequeueTask();
                task?.Invoke();
            }
        }

        public void EnqueueTask(Action task, int priority)
        {
            _taskQueue.EnqueueTask(task, priority);
        }
    }
}
