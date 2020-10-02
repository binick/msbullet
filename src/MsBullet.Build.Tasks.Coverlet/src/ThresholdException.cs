// See the LICENSE.TXT file in the project root for full license information.

using System;

namespace MsBullet.Build.Tasks.Coverlet
{
    [Serializable]
    public class ThresholdException : Exception
    {
        public ThresholdException()
        {
        }

        public ThresholdException(string message)
            : base(message)
        {
        }

        public ThresholdException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ThresholdException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
}
