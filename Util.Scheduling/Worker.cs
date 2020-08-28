using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace Util.Scheduling
{
    /// <summary>表示将重复执行特定方法的后台工作者
    /// </summary>
    public class Worker
    {
        private readonly string _actionName;
        private readonly Action _action;
        private WorkerState _currentState;

        /// <summary>Returns the action name of the current worker.
        /// </summary>
        public string ActionName
        {
            get { return _actionName; }
        }

        /// <summary>Initialize a new worker with the specified action.
        /// </summary>
        /// <param name="actionName">The action name.</param>
        /// <param name="action">The action to run by the worker.</param>
        public Worker(string actionName, Action action)
        {
            _actionName = actionName;
            _action = action;
        }

        /// <summary>Start the worker if it is not running.
        /// </summary>
        public Worker Start()
        {
            if (_currentState != null && !_currentState.StopRequested) return this;

            var thread = new Thread(Loop)
            {
                Name = string.Format("{0}.Worker", _actionName),
                IsBackground = true
            };
            var state = new WorkerState();

            thread.Start(state);

            _currentState = state;
           

            return this;
        }
        /// <summary>Request to stop the worker.
        /// </summary>
        public Worker Stop()
        {
            if (_currentState == null) return this;

            _currentState.StopRequested = true;
         
            return this;
        }

        private void Loop(object data)
        {
            var state = (WorkerState)data;

            while (!state.StopRequested)
            {
                try
                {
                    _action();
                }
                catch (ThreadAbortException)
                {
                  
                    Thread.ResetAbort();
                 
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format((string.Format("Worker action has exception, actionName:{0}", _actionName)), ex));
                }
            }
        }
        private static int GetNativeThreadId(Thread thread)
        {
            var f = typeof(Thread).GetField("DONT_USE_InternalThread", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
            var pInternalThread = (IntPtr)f.GetValue(thread);
            var nativeId = Marshal.ReadInt32(pInternalThread, (IntPtr.Size == 8) ? 548 : 348);
            return nativeId;
        }

        class WorkerState
        {
            public string Id = ObjectId.GenerateNewStringId();
            public bool StopRequested;
        }
    }
}
