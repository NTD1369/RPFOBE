using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RPFO.API.Helpers
{
   
    public class SemaphoreWaiter : IDisposable
    { 
        Semaphore Slim;
        Semaphore LinuxDockerSemaphore = new Semaphore(12, 12); 
        public SemaphoreWaiter WaitForLinuxDocker(int timeoutMS = -1)
        {
            return new SemaphoreWaiter(LinuxDockerSemaphore, timeoutMS);
        }
        public SemaphoreWaiter(int timeoutMS = -1)
        {
            Slim = new Semaphore(12, 12);
            Slim.WaitOne(timeoutMS);
        }
        public SemaphoreWaiter(Semaphore slim, int timeoutMS = -1)
        {
            Slim = slim;
            Slim.WaitOne(timeoutMS);
        }

        public void Dispose()
        {
            Slim.Release();
        }
    }
}
