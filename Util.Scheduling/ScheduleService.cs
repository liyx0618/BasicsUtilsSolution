using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace Util.Scheduling
{
    public class ScheduleService : IScheduleService
    {
        private readonly ConcurrentDictionary<int, TimerBasedTask> _taskDict = new ConcurrentDictionary<int, TimerBasedTask>();

        private int _maxTaskId;

        public ScheduleService()
        {

        }

        public int ScheduleTask(string actionName, Action action, int dueTime, int period)
        {
            var newTaskId = Interlocked.Increment(ref _maxTaskId);
            var timer = new Timer((obj) =>
            {
                var currentTaskId = (int)obj;
                TimerBasedTask currentTask;
                if (_taskDict.TryGetValue(currentTaskId, out currentTask))
                {
                    if (currentTask.Stoped)
                    {
                        return;
                    }

                    try
                    {
                        currentTask.Timer.Change(Timeout.Infinite, Timeout.Infinite);
                        if (currentTask.Stoped)
                        {
                            return;
                        }
                        currentTask.Action();
                    }
                    catch (ObjectDisposedException) { }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Task has exception, actionName:{0}, dueTime:{1}, period:{2},ex:{3}", currentTask.ActionName, currentTask.DueTime, currentTask.Period, ex.Message));
                    }
                    finally
                    {
                        if (!currentTask.Stoped)
                        {
                            try
                            {
                                currentTask.Timer.Change(currentTask.Period, currentTask.Period);
                            }
                            catch (ObjectDisposedException) { }
                        }
                    }
                }
            }, newTaskId, Timeout.Infinite, Timeout.Infinite);

            if (!_taskDict.TryAdd(newTaskId, new TimerBasedTask { ActionName = actionName, Action = action, Timer = timer, DueTime = dueTime, Period = period, Stoped = false }))
            {
                throw new Exception(string.Format("Schedule task failed, actionName:{0}, dueTime:{1}, period:{2}", actionName, dueTime, period));
                //return -1;
            }

            timer.Change(dueTime, period);
            Console.WriteLine(string.Format("计划任务成功, actionName:{0}, dueTime:{1}, period:{2}", actionName, dueTime, period));

            return newTaskId;
        }

        public void ShutdownTask(int taskId)
        {
            TimerBasedTask task;
            if (_taskDict.TryRemove(taskId, out task))
            {
                task.Stoped = true;
                task.Timer.Change(Timeout.Infinite, Timeout.Infinite);
                task.Timer.Dispose();
                Console.WriteLine(string.Format("Shutdown task success, actionName:{0}, dueTime:{1}, period:{2}", task.ActionName, task.DueTime, task.Period));
            }
        }

        class TimerBasedTask
        {
            public string ActionName;
            public Action Action;
            public Timer Timer;
            public int DueTime;
            public int Period;
            public bool Stoped;
        }
    }
}
