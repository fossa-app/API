using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using TIKSN.Mapping;

namespace Fossa.API.Web.Messages.Commands;

public class EmployeeManagementApiCommandHandler : ApiMessageHandler<EmployeeId, EmployeeManagementApiCommand, Unit, EmployeeManagementCommand, Unit>
{
  private readonly IMapper<long, BranchId> _branchDataIdentityToDomainIdentityMapper;
  private readonly IMapper<long, DepartmentId> _departmentDataIdentityToDomainIdentityMapper;

  public EmployeeManagementApiCommandHandler(
      ISender sender,
      ITenantIdProvider<Guid> tenantIdProvider,
      IUserIdProvider<Guid> userIdProvider,
      IMapper<EmployeeId, long> domainIdentityToDataIdentityMapper,
      IMapper<long, EmployeeId> dataIdentityToDomainIdentityMapper,
      IMapper<long, BranchId> branchDataIdentityToDomainIdentityMapper,
      IMapper<long, DepartmentId> departmentDataIdentityToDomainIdentityMapper)
      : base(
          sender,
          tenantIdProvider,
          userIdProvider,
          domainIdentityToDataIdentityMapper,
          dataIdentityToDomainIdentityMapper)
  {
    _branchDataIdentityToDomainIdentityMapper = branchDataIdentityToDomainIdentityMapper;
    _departmentDataIdentityToDomainIdentityMapper = departmentDataIdentityToDomainIdentityMapper;
  }

  protected override Unit MapToApiResponse(Unit domainResponse) => domainResponse;

  protected override EmployeeManagementCommand MapToDomainRequest(EmployeeManagementApiCommand apiRequest)
  {
    var tenantId = _tenantIdProvider.GetTenantId();
    var userId = _userIdProvider.GetUserId();
    return new EmployeeManagementCommand(
      _dataIdentityToDomainIdentityMapper.Map(apiRequest.Id),
      tenantId,
      userId,
      Optional(apiRequest.AssignedBranchId).Map(_branchDataIdentityToDomainIdentityMapper.Map),
      Optional(apiRequest.AssignedDepartmentId).Map(_departmentDataIdentityToDomainIdentityMapper.Map),
      Optional(apiRequest.ReportsToId).Map(_dataIdentityToDomainIdentityMapper.Map));
  }
}
