using Confluent.Kafka;
using Fossa.Messaging;

namespace Fossa.API.FunctionalTests;

public class TestProducerProvider : IProducerProvider
{
  public IProducer<string?, byte[]> GetProducer()
  {
    return new TestKafkaProducer();
  }
}
