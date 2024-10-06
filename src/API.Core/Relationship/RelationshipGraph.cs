using System.Collections.Frozen;
using System.Reflection;
using Fossa.API.Core.Repositories;
using LanguageExt;
using LanguageExt.ClassInstances.Pred;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Data;
using static LanguageExt.Prelude;

namespace Fossa.API.Core.Relationship;

public partial class RelationshipGraph : IRelationshipGraph
{
  private const string _entityIdName = nameof(IEntity<int>.ID);
  private static readonly Lazy<FrozenDictionary<Type, TypeInfo>> _entityIdTypeToOwnerEntityTypeMap = new(CreateEntityIdTypeToOwnerEntityTypeMap, LazyThreadSafetyMode.ExecutionAndPublication);
  private static readonly Lazy<FrozenDictionary<Type, Seq<PropertyInfo>>> _entityIdTypeToReferenceIdPropertiesMap = new(CreateEntityIdTypeToReferenceIdPropertiesMap, LazyThreadSafetyMode.ExecutionAndPublication);
  private static readonly Type _entityInterfaceType = typeof(IEntity<>);
  private static readonly Lazy<Seq<TypeInfo>> _entityTypes = new(CreateEntityTypes, LazyThreadSafetyMode.ExecutionAndPublication);
  private static readonly Lazy<FrozenDictionary<Type, Lst<NonEmpty, TypeInfo>>> _entityTypeToQueryRepositoryTypesMap = new(CreateEntityTypeToQueryRepositoryTypesMap, LazyThreadSafetyMode.ExecutionAndPublication);
  private static readonly Type _genericDependencyQueryRepositoryInterfaceType = typeof(IDependencyQueryRepository<>);
  private static readonly Type _genericQueryRepositoryInterfaceType = typeof(IQueryRepository<,>);

  private readonly IServiceProvider _serviceProvider;

  public RelationshipGraph(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
  }

  public async Task<bool> HasDependentEntitiesAsync<T>(T id, CancellationToken cancellationToken) where T : IEquatable<T>
  {
    var dependencyQueryRepositoryInterfaceType = _genericDependencyQueryRepositoryInterfaceType.MakeGenericType(id.GetType());
    var hasDependencyAsyncMethodInfo = dependencyQueryRepositoryInterfaceType.GetMethods().Single()
      ?? throw new InvalidOperationException("HasDependencyAsync MethodInfo should not be null.");
    var propertyInfos = ListReferenceIdProperties(id);
    var dependencyQueryRepositoryTypes = propertyInfos.Map(x =>
    {
      var declaringType = (x?.DeclaringType)
        ?? throw new InvalidOperationException("Property Declared Type should not be null.");

      var queryRepositoryTypes = _entityTypeToQueryRepositoryTypesMap.Value[declaringType];
      return queryRepositoryTypes.Filter(x => x.IsAssignableTo(dependencyQueryRepositoryInterfaceType));
    }).Flatten().ToSeq().Map(x => x.AsType());

    var dependencyQueryRepositories = dependencyQueryRepositoryTypes.Map(_serviceProvider.GetRequiredService);

    var hasDependencyAsyncMethodResults = dependencyQueryRepositories.Map(x => hasDependencyAsyncMethodInfo.Invoke(x, [id, cancellationToken])).Cast<Task<bool>>();

    var results = await Task.WhenAll(hasDependencyAsyncMethodResults).ConfigureAwait(false);
    return results.Contains(true);
  }

  private static FrozenDictionary<Type, TypeInfo> CreateEntityIdTypeToOwnerEntityTypeMap()
  {
    return _entityTypes.Value.ToFrozenDictionary(k => k.GetDeclaredProperty(_entityIdName)!.PropertyType, v => v);
  }

  private static FrozenDictionary<Type, Seq<PropertyInfo>> CreateEntityIdTypeToReferenceIdPropertiesMap()
  {
    return _entityTypes.Value
      .SelectMany(x => x.DeclaredProperties)
      .Where(p => !p.Name.Equals(_entityIdName, StringComparison.Ordinal) && _entityIdTypeToOwnerEntityTypeMap.Value.ContainsKey(p.PropertyType))
      .GroupBy(x => x.PropertyType)
      .ToFrozenDictionary(k => k.Key, v => v.ToSeq());
  }

  private static Seq<TypeInfo> CreateEntityTypes()
  {
    return typeof(RelationshipGraph).Assembly.DefinedTypes.Where(IsEntityType).ToSeq();
  }

  private static FrozenDictionary<Type, Lst<NonEmpty, TypeInfo>> CreateEntityTypeToQueryRepositoryTypesMap()
  {
    var interfaceTypes = typeof(RelationshipGraph).Assembly.DefinedTypes.Where(x => x.IsInterface).ToSeq();

    return _entityIdTypeToOwnerEntityTypeMap.Value
      .Select(x => (interfaceType: _genericQueryRepositoryInterfaceType.MakeGenericType(x.Value, x.Key), entityType: x.Value))
      .ToFrozenDictionary(k => k.entityType.AsType(), v => new Lst<NonEmpty, TypeInfo>(interfaceTypes.Where(i => i.IsAssignableTo(v.interfaceType))));
  }

  private static bool IsEntityType(TypeInfo typeInfo)
  {
    return typeInfo.ImplementedInterfaces.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == _entityInterfaceType);
  }

  private static Seq<PropertyInfo> ListReferenceIdProperties<T>(T id) where T : IEquatable<T>
  {
    var idType = id.GetType();
    if (_entityIdTypeToReferenceIdPropertiesMap.Value.TryGetValue(idType, out var referenceIdProperties))
    {
      return referenceIdProperties;
    }

    return Seq<PropertyInfo>();
  }
}
