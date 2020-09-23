# Observability

Ths project implements observability by providing a couple abstractions:

- Simple core interfaces abstract away the telemetry/metrics implementation (Null pattern versions can be provided to make noop)
  - `ICoreTelemetry`
  - `ISpanActivity`
  - TODO: Metrics
  - TODO: Exceptions

Specific adapters can be configured for any tech stack.

- Typed observability interfaces injected and used by services
  - Example: `IWeatherForecasterObservability`
  - Example: `ISampleObservability`

Services code to these interfaces and they don't need to change when the underlying tech changes.

Currently this sample is foucusing on exploring activities.  The goal would be that you can get the same type of view in either Jeager or Application Insights.

## Application Insights

TODO: Audience likely understands this, will fill out later

## Jaeger

This sample uses the local 'All in one' docker docker image to test.  Instructions [here](https://www.jaegertracing.io/docs/1.19/getting-started/#all-in-one)

You can run this command below to have it up in the background.

```bash
$ docker run -d --name jaeger \
  -e COLLECTOR_ZIPKIN_HTTP_PORT=9411 \
  -p 5775:5775/udp \
  -p 6831:6831/udp \
  -p 6832:6832/udp \
  -p 5778:5778 \
  -p 16686:16686 \
  -p 14268:14268 \
  -p 14250:14250 \
  -p 9411:9411 \
  jaegertracing/all-in-one:1.19
```

Then navigate to [http://localhost:16686/](http://localhost:16686/) to access the UI.  After running the sample webapi locally you should be able to view spans if coded and configured correctly.
