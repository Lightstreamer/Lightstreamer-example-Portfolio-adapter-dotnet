#region License
/*
* Copyright (c) Lightstreamer Srl
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
#endregion License

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Lightstreamer.Adapters.PortfolioDemo
{
    /// <summary>
    /// Used to provide an executor of tasks in a single dedicated thread.
    /// More complete implementations of this service
    /// may be provided by standard tool libraries.
    /// </summary>
    class NotificationQueue
    {
        public delegate void Notify();

        private bool closed = false;

        private List<Notify> myList = new List<Notify>();
        private ManualResetEvent available = new ManualResetEvent(false);

        public void Add(Notify fun)
        {
            lock (this)
            {
                if (!closed)
                {
                    myList.Add(fun);
                    available.Set();
                }
            }
        }

        public void Start()
        {
            Thread t = new Thread(dequeue);
            t.IsBackground = true;
            t.Start();
        }

        public void End()
        {
            lock (this)
            {
                closed = true;
                available.Set();
            }
        }

        void dequeue()
        {
            while (true)
            {
                Notify fun = null;
                lock (this)
                {
                    if (myList.Count > 0)
                    {
                        fun = myList[0];
                        myList.RemoveAt(0);
                    }
                    else if (closed)
                    {
                        return;
                    }
                    else
                    {
                        available.Reset();
                    }
                }
                if (fun != null)
                {
                    try
                    {
                        fun();
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    available.WaitOne();
                }
            }
        }
    }
}
