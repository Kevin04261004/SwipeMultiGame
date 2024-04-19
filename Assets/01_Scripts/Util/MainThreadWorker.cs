using System;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class MainThreadWorker : MonoBehaviour
    {
        public static MainThreadWorker Instance { get; private set; }
        public readonly Queue<Action> ActionQueue = new Queue<Action>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            while (ActionQueue.Count > 0)
            {
                ActionQueue.Dequeue().Invoke();
            }
        }

        public void EnqueueJob(Action action)
        {
            ActionQueue.Enqueue(action);   
        }
    }
}
