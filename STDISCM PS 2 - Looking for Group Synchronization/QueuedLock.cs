using System;
using System.Collections.Generic;
using System.Threading;

public class QueuedLock
{
    private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
    private readonly Queue<TaskCompletionSource<bool>> queue = new Queue<TaskCompletionSource<bool>>();
    private readonly object queueLock = new object();

    public IDisposable Lock()
    {
        var tcs = new TaskCompletionSource<bool>();
        lock (queueLock)
        {
            queue.Enqueue(tcs);
            if (queue.Count == 1)
            {
                tcs.SetResult(true);
            }
        }

        tcs.Task.Wait();
        semaphore.Wait();

        return new Releaser(this);
    }

    private void Release()
    {
        semaphore.Release();
        lock (queueLock)
        {
            queue.Dequeue();
            if (queue.Count > 0)
            {
                queue.Peek().SetResult(true);
            }
        }
    }

    private class Releaser : IDisposable
    {
        private readonly QueuedLock queuedLock;

        public Releaser(QueuedLock queuedLock)
        {
            this.queuedLock = queuedLock;
        }

        public void Dispose()
        {
            queuedLock.Release();
        }
    }
}
