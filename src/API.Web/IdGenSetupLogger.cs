using IdGen;

namespace Fossa.API.Web;

public class IdGenSetupLogger
{
  private readonly IdGenerator _idGenerator;
  private readonly ILogger<IdGenSetupLogger> _logger;

  public IdGenSetupLogger(
    IdGenerator idGenerator,
    ILogger<IdGenSetupLogger> logger)
  {
    _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  public void LogIdGenSetup()
  {
    using (_logger.BeginScope("IdGen Setup"))
    {
      _logger.LogInformation("IdGen: Epoch : {Epoch}", _idGenerator.Options.TimeSource.Epoch);
      _logger.LogInformation("IdGen: Max. generators : {MaximumGenerators}", _idGenerator.Options.IdStructure.MaxGenerators);
      _logger.LogInformation("IdGen: Id's/ms per generator : {IdsPerMillisecondsPerGenerator}", _idGenerator.Options.IdStructure.MaxSequenceIds);
      _logger.LogInformation("IdGen: Id's/ms total : {IdsPerMillisecondsIntotal}", _idGenerator.Options.IdStructure.MaxGenerators * _idGenerator.Options.IdStructure.MaxSequenceIds);
      _logger.LogInformation("IdGen: Wraparound interval : {WraparoundInterval}", _idGenerator.Options.IdStructure.WraparoundInterval(_idGenerator.Options.TimeSource));
      _logger.LogInformation("IdGen: Wraparound date : {WraparoundDate}", _idGenerator.Options.IdStructure.WraparoundDate(_idGenerator.Options.TimeSource.Epoch, _idGenerator.Options.TimeSource));
      _logger.LogInformation("IdGen: Current Generator ID : {GeneratorId}", _idGenerator.Id);
      _logger.LogInformation("IdGen: Next ID : {NextID}", _idGenerator.CreateId());
      _logger.LogInformation("IdGen: MaxLong : {MaxLong}", long.MaxValue);
    }
  }
}
