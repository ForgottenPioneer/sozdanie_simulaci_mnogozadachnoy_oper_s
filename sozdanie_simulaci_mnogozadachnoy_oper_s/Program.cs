using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class Process
{
    public int Id { get; set; }
    public int BurstTime { get; set; }
    public int Priority { get; set; }

    public Process(int id, int burstTime, int priority)
    {
        Id = id;
        BurstTime = burstTime;
        Priority = priority;
    }
}

public class ProcessScheduler
{
    private Queue<Process> processQueue = new Queue<Process>();
    private List<Thread> threads = new List<Thread>();

    public void AddProcess(Process process)
    {
        processQueue.Enqueue(process);
    }

    public void RunFIFO()
    {
        Console.WriteLine("Running FIFO...");
        while (processQueue.Count > 0)
        {
            Process process = processQueue.Dequeue();
            Console.WriteLine($"Process {process.Id} started execution");
            Thread.Sleep(process.BurstTime * 1000);
            Console.WriteLine($"Process {process.Id} has completed");
        }
        Console.WriteLine("FIFO completed.");
    }

    public void RunRoundRobin()
    {
        Console.WriteLine("Running Round Robin...");
        int timeQuantum = 2;
        while (processQueue.Count > 0)
        {
            Process process = processQueue.Dequeue();
            Console.WriteLine($"Process {process.Id} started execution");
            for (int i = 0; i < process.BurstTime; i += timeQuantum)
            {
                Thread.Sleep(timeQuantum * 1000);
                Console.WriteLine($"Process {process.Id} is running (burst time remaining: {process.BurstTime - i})");
            }
            Console.WriteLine($"Process {process.Id} has completed");
        }
        Console.WriteLine("Round Robin completed.");
    }

    public void RunPriorityScheduling()
    {
        Console.WriteLine("Running Priority Scheduling...");
        List<Process> processes = new List<Process>(processQueue);
        processes.Sort((p1, p2) => p2.Priority.CompareTo(p1.Priority));
        foreach (Process process in processes)
        {
            Console.WriteLine($"Process {process.Id} started execution");
            Thread.Sleep(process.BurstTime * 1000);
            Console.WriteLine($"Process {process.Id} has completed");
        }
        Console.WriteLine("Priority Scheduling completed.");
    }

    public void RunProcesses()
    {
        Console.WriteLine("Running processes...");
        List<Task> tasks = new List<Task>();
        while (processQueue.Count > 0)
        {
            Process process = processQueue.Dequeue();
            Task task = Task.Run(() =>
            {
                Console.WriteLine($"Process {process.Id} started execution");
                Thread.Sleep(process.BurstTime * 1000);
                Console.WriteLine($"Process {process.Id} has completed");
            });
            tasks.Add(task);
            threads.Add(new Thread(() => task.Wait())); 
        }
        Task.WaitAll(tasks.ToArray());
        Console.WriteLine("All processes started.");
    }

    public void DisplaySystemState()
    {
        Console.WriteLine("System state:");
        foreach (Thread thread in threads)
        {
            Console.WriteLine($"Thread {thread.ManagedThreadId} is {thread.ThreadState}");
        }
    }

    public void TerminateAllProcesses()
    {
        Console.WriteLine("Terminating all processes...");
        foreach (Thread thread in threads)
        {
            thread.Abort();
        }
        threads.Clear();
        Console.WriteLine("All processes terminated.");
    }
}

class Program
{
    static void Main(string[] args)
    {
        ProcessScheduler scheduler = new ProcessScheduler();

        scheduler.AddProcess(new Process(1, 5, 1));
        scheduler.AddProcess(new Process(2, 3, 2));
        scheduler.AddProcess(new Process(3, 7, 1));

        scheduler.RunFIFO();
        scheduler.DisplaySystemState();

        scheduler.RunRoundRobin();
        scheduler.DisplaySystemState();

        scheduler.RunPriorityScheduling();
        scheduler.DisplaySystemState();

        scheduler.RunProcesses();
        scheduler.DisplaySystemState();

        scheduler.TerminateAllProcesses();
    }
}