
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace StationeersWebApi
{

    internal class QueuedTask
    {
        public Func<object> function;
        public TaskCompletionSource<object> completionSource;
    }

    // From: https://answers.unity.com/questions/305882/how-do-i-invoke-functions-on-the-main-thread.html
    public class Dispatcher : MonoBehaviour
    {
        public static Task RunOnMainThread(Action action)
        {
            Logging.Log("Dispatcher item added");
            return Dispatcher.RunOnMainThread<object>(() =>
            {
                action();
                return null;
            });
        }

        public static Task<T> RunOnMainThread<T>(Func<T> function)
        {
            Logging.Log("Dispatcher item added");
            var source = new TaskCompletionSource<object>();
            var queueItem = new QueuedTask()
            {
                function = () => function(),
                completionSource = source
            };

            lock (_backlog)
            {
                _backlog.Add(queueItem);
                _queued = true;
            }

            // Sigh...
            return source.Task.ContinueWith(t => (T)t.Result);
        }

        public static void Initialize()
        {
            if (_instance == null)
            {
                _instance = new GameObject("Dispatcher").AddComponent<Dispatcher>();
                DontDestroyOnLoad(_instance.gameObject);
            }
        }

        private void Update()
        {
            if (_queued)
            {
                lock (_backlog)
                {
                    var tmp = _actions;
                    _actions = _backlog;
                    _backlog = tmp;
                    _queued = false;
                }

                foreach (var action in _actions)
                {
                    Logging.Log("Dispatcher item drained");
                    try
                    {
                        var result = action.function();
                        action.completionSource.TrySetResult(result);
                    }
                    catch (Exception e)
                    {
                        action.completionSource.TrySetException(e);
                    }
                }

                _actions.Clear();
            }
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        static Dispatcher _instance;
        static volatile bool _queued = false;
        static List<QueuedTask> _backlog = new List<QueuedTask>(8);
        static List<QueuedTask> _actions = new List<QueuedTask>(8);
    }
}