using System;
using System.Collections.Generic;
using System.Text;
using Qed.Webhook.JobMaster.Interfaces;

namespace Qed.JobPicker.Worker.Configurations
{
    public class JobPickerConfiguration : IJobMasterConfiguration
    {
        private readonly int _wokerId;

        public JobPickerConfiguration(int workerId)
        {
            _wokerId = workerId;
        }

        public int GetWorkerId()
        {
            return _wokerId;
        }
    }
}
