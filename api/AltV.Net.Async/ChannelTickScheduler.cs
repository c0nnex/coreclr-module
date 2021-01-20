using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AltV.Net.Async
{
    internal class ChannelTickScheduler : TaskScheduler , ITickScheduler
    {
        private readonly Thread mainThread;

        public override int MaximumConcurrencyLevel { get; } = 1;

        private readonly Channel<Task> tasks = Channel.CreateUnbounded<Task>(new UnboundedChannelOptions
            {SingleReader = true});

        private Task currentTask;

        private readonly ChannelReader<Task> reader;

        private readonly ChannelWriter<Task> writer;
        
        private readonly TaskFactory taskFactory;

        public ChannelTickScheduler(Thread mainThread)
        {
            this.mainThread = mainThread;
            reader = tasks.Reader;
            writer = tasks.Writer;
            taskFactory = new TaskFactory(
                CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                TaskContinuationOptions.None, this);
        }

        protected override IEnumerable<Task> GetScheduledTasks() => null;

        protected override void QueueTask(Task task) => writer.WriteAsync(task);

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) =>
            Thread.CurrentThread == mainThread && TryExecuteTask(task);

        public void Schedule(Action action)
        {
            taskFactory.StartNew(action);
        }

        public void Schedule(Action<object> action, object state)
        {
            taskFactory.StartNew(action, state);
        }

        public Task ScheduleTask(Action action)
        {
            return taskFactory.StartNew(action);
        }

        public Task ScheduleTask(Action<object> action, object state)
        {
            return taskFactory.StartNew(action, state);
        }

        public Task<TResult> ScheduleTask<TResult>(Func<TResult> action)
        {
            return taskFactory.StartNew(action);
        }

        public Task<TResult> ScheduleTask<TResult>(Func<object, TResult> action, object value)
        {
            return taskFactory.StartNew(action, value);
        }
        
        public void Tick()
        {
            while (reader.TryRead(out currentTask))
            {
                TryExecuteTask(currentTask);
            }
        }
    }
}