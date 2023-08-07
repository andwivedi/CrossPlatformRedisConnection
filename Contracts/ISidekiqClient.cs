using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    internal interface ISidekiqClient
    {
        /// <summary>
        /// Enqueues a job in the Job Queue system.
        /// </summary>
        /// <param name="SidekiqJobOptions">Options and metadata for the job in the form of a dictionary with string keys and object values.</param>
        /// <param name="JobParameters">Parameters required to perform the job in the form of a dictionary with string keys and object values.</param>
        /// <param name="ClassName">The name of the class that contains the logic to execute the job.</param>
        void Enqueue(Dictionary<string, object> SidekiqJobOptions, Dictionary<string, object> JobParameters, string ClassName);

        /// Additional functionality like EnqueueIn similar to perform_in of Sidekiq can be added here.
    }
}
