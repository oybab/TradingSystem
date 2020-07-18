using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Oybab.ServerManager.Model.Models
{
    internal class BackgroundJobSchedueller
    {
        readonly BlockingCollection<Action> _queue;
        readonly Thread _thread;

        public BackgroundJobSchedueller()
        {
            _queue = new BlockingCollection<Action>();
            _thread = new Thread(WorkThread)
            {
                IsBackground = true,
                Name = "Background Queue Processor"
            };
            _thread.Start();
        }

        internal void StopSchedueller()
        {
            //Tell GetConsumingEnumerable() to let the user out of the foreach loop
            // once the collection is empty.
            _queue.CompleteAdding();

            //Wait for the foreach loop to finish processing.
            _thread.Join();
        }

        internal void QueueJob(Action job)
        {
            _queue.Add(job);
        }

        private void WorkThread()
        {
            foreach (var action in _queue.GetConsumingEnumerable())
            {
                try
                {
                    action();
                }
                catch
                {
                    //Do something with the exception here
                }
            }
        }
    }
}
