namespace eda7k.Models.MyChannel
{
    public enum Operations
    {
        Add,
        Delete
    }
    public static class MyChannel
    {
        private class MyDispatcher : IDisposable
        {
            private SemaphoreSlim _semaphore;
            public MyDispatcher(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore; 
            }

            public void Dispose()
            {
                _semaphore.Release();
            }
        }

        private static SemaphoreSlim _sync = new SemaphoreSlim(1);
        private static SemaphoreSlim _semaphoreTaskAction = new SemaphoreSlim(0);
        private static SemaphoreSlim _semaphoreReadWrite = new SemaphoreSlim(1);
        private static Task<KeyValuePair<Operations, int>> _sheduleTask = SheduleTask();
        private static KeyValuePair<Operations, int> _operationAndId;
        public static async Task WriteAsync(KeyValuePair<Operations, int> operationAndId)
        {
            await _semaphoreReadWrite.WaitAsync();
            _operationAndId = operationAndId;
            _semaphoreTaskAction.Release();
            await _sync.WaitAsync();
            await Task.Delay(100);//За это время все отложенные таски должны завершиться
            _sync.Release();
            await _semaphoreTaskAction.WaitAsync();
            _semaphoreReadWrite.Release();
        }
        private static async Task<KeyValuePair<Operations, int>> SheduleTask()
        {
            await _semaphoreTaskAction.WaitAsync();
            using (MyDispatcher myDispatcher = new MyDispatcher(_semaphoreTaskAction))
            {
                return _operationAndId;
            }
        }
        public static async Task<KeyValuePair<Operations, int>> GetSheduleTask()
        {
            await _sync.WaitAsync();
            if (_sheduleTask.IsCompleted)
            {
                _sheduleTask = SheduleTask();
            }
            _sync.Release();
            return await _sheduleTask;
        }
    }
}
