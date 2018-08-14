using System;
using System.Runtime.Serialization;

namespace Workflow.Core
{
    [Serializable]
    public class HyperVolumeConfigurationException : Exception
    {
        public HyperVolumeConfigurationException() { }
        public HyperVolumeConfigurationException(string message) : base(message) { }
        public HyperVolumeConfigurationException(string message, Exception inner) : base(message, inner) { }

        protected HyperVolumeConfigurationException(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }
    }
}
