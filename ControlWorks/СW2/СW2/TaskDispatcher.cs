using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class TaskDispatcher
{
    private readonly Semaphore _semaphore;
    private readonly List<Action> _tasks;
    
    public TaskDispatcher(int maxConcurrentTasks = 5)
    {
        _semaphore = new Semaphore(maxConcurrentTasks, maxConcurrentTasks);
        _tasks = new List<Action>();
    }
    
    public void AddTask(Action task)
    {
        _tasks.Add(task);
    }
    
    public void ExecuteAll()
    {
        List<Task> runningTasks = new List<Task>();
        
        foreach (var task in _tasks)
        {
            
            _semaphore.WaitOne();
            
            var t = Task.Run(() =>
            {
                try
                {
                    task?.Invoke();
                }
                finally
                {
                    _semaphore.Release();
                }
            });
            
            runningTasks.Add(t);
        }
        
        Task.WaitAll(runningTasks.ToArray());
    }
}


class Program
{
    static void Main()
    {
        var dispatcher = new TaskDispatcher(maxConcurrentTasks: 5);
        

        for (int i = 0; i < 50; i++)
        {
            int taskId = i;
            dispatcher.AddTask(() =>
            {
                Console.WriteLine($"Task {taskId} started on thread {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(100); 
                Console.WriteLine($"Task {taskId} completed");
            });
        }
        
        dispatcher.ExecuteAll();
        Console.WriteLine("All tasks completed");
    }
}