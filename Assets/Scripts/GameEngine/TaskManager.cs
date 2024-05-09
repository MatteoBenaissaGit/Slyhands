using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameEngine
{
    public class TaskManager
    {
        public bool IsDoingTask => _taskQueue.Count > 0;

        private Queue<Func<Task>> _taskQueue = new Queue<Func<Task>>();
        private Task _currentTask;
        
        public void UpdateTaskQueue()
        {
            if (_currentTask == null || _currentTask.IsCompleted == false)
            {
                return;
            }
            
            _taskQueue.Dequeue();
            _currentTask = null;
            LaunchNextTask();
        }

        private void LaunchNextTask()
        {
            if (_taskQueue.Count <= 0)
            {
                return;
            }

            _currentTask = _taskQueue.Peek()();
        }

        public void EnqueueTask(Func<Task> task)
        {
            _taskQueue.Enqueue(task);
            if (_taskQueue.Count == 1)
            {
                LaunchNextTask();
            }
        }

        public async Task PlayActionInSeconds(float seconds, Action action)
        {
            int milliseconds = (int)(seconds * 1000f);
            await Task.Delay(milliseconds);
            
            action.Invoke();
        }
    }
}