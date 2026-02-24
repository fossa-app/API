using Confluent.Kafka;

namespace Fossa.API.FunctionalTests;

#pragma warning disable S3881 // "IDisposable" should be implemented correctly
public class TestKafkaProducer : IProducer<string?, byte[]>
#pragma warning restore S3881 // "IDisposable" should be implemented correctly
{
  public Handle Handle => throw new NotImplementedException();

  public string Name => throw new NotImplementedException();

  public void AbortTransaction(TimeSpan timeout)
  {
    throw new NotImplementedException();
  }

  public void AbortTransaction()
  {
    throw new NotImplementedException();
  }

  public int AddBrokers(string brokers)
  {
    throw new NotImplementedException();
  }

  public void BeginTransaction()
  {
    throw new NotImplementedException();
  }

  public void CommitTransaction(TimeSpan timeout)
  {
    throw new NotImplementedException();
  }

  public void CommitTransaction()
  {
    throw new NotImplementedException();
  }

  public void Dispose()
  {
    throw new NotImplementedException();
  }

  public int Flush(TimeSpan timeout)
  {
    throw new NotImplementedException();
  }

  public void Flush(CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public void InitTransactions(TimeSpan timeout)
  {
    throw new NotImplementedException();
  }

  public int Poll(TimeSpan timeout)
  {
    throw new NotImplementedException();
  }

  public void Produce(string topic, Message<string?, byte[]> message, Action<DeliveryReport<string?, byte[]>>? deliveryHandler = null)
  {
    throw new NotImplementedException();
  }

  public void Produce(TopicPartition topicPartition, Message<string?, byte[]> message, Action<DeliveryReport<string?, byte[]>>? deliveryHandler = null)
  {
    throw new NotImplementedException();
  }

  public Task<DeliveryResult<string?, byte[]>> ProduceAsync(string topic, Message<string?, byte[]> message, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<DeliveryResult<string?, byte[]>> ProduceAsync(TopicPartition topicPartition, Message<string?, byte[]> message, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public void SendOffsetsToTransaction(IEnumerable<TopicPartitionOffset> offsets, IConsumerGroupMetadata groupMetadata, TimeSpan timeout)
  {
    throw new NotImplementedException();
  }
}
