using Fossa.API.Web.ApiModels;
using TIKSN.Data;
using TIKSN.Mapping;

namespace Fossa.API.Web.Mappers;

public class PagingResponseModelMapper<TEntity, TModel> :
  IMapper<PageResult<TEntity>, PagingResponseModel<TModel>>,
  IMapper<Seq<TEntity>, PagingResponseModel<TModel>>
{
  private readonly IMapper<TEntity, TModel> _itemMapper;

  public PagingResponseModelMapper(IMapper<TEntity, TModel> itemMapper)
  {
    _itemMapper = itemMapper ?? throw new ArgumentNullException(nameof(itemMapper));
  }

  public PagingResponseModel<TModel> Map(PageResult<TEntity> source)
  {
    return new PagingResponseModel<TModel>(
      source.Page.Number, source.Page.Size,
      [.. source.Items.Select(_itemMapper.Map)],
      source.TotalItems.ToNullable(),
      source.TotalPages.ToNullable());
  }

  public PagingResponseModel<TModel> Map(Seq<TEntity> source)
  {
    return new PagingResponseModel<TModel>(
      null, null,
      [.. source.Select(_itemMapper.Map)],
      null,
      null);
  }
}
