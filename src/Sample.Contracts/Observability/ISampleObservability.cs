using System;

namespace Sample.Observability
{
    public interface ISampleObservability
    {
        IDisposable StartOperation(int depth, int sequence);

        IDisposable StartSubOperation(int sequenceId);
    }
}
