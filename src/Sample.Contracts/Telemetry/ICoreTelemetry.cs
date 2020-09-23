namespace Sample.Telemetry
{
    public interface ICoreTelemetry
    {
        ISpanActivity StartSpanActivity(string name);
    }
}
