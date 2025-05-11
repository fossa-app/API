using Fossa.API.Core.Entities;
using Fossa.API.Core.Messages.Commands;
using Fossa.API.Core.Tenant;
using Fossa.API.Core.User;
using TIKSN.Mapping;
using static LanguageExt.Prelude;
using TIKSN.Data;

namespace Fossa.API.Web.Messages.Commands;

public class DepartmentCreationApiCommandHandler :
    ApiMessageHandler<DepartmentId, DepartmentCreationApiCommand, Unit, DepartmentCreationCommand, Unit>
{
    private readonly IMapper<long, DepartmentId> _departmentDataIdentityToDomainIdentityMapper;
    private readonly IMapper<long, EmployeeId> _employeeDataIdentityToDomainIdentityMapper;

    public DepartmentCreationApiCommandHandler(
        ISender sender,
        ITenantIdProvider<Guid> tenantIdProvider,
        IUserIdProvider<Guid> userIdProvider,
        IMapper<long, DepartmentId> departmentDataIdentityToDomainIdentityMapper,
        IMapper<long, EmployeeId> employeeDataIdentityToDomainIdentityMapper,
        IMapper<DepartmentId, long> domainIdentityToDataIdentityMapper)
        : base(
            sender,
            tenantIdProvider,
            userIdProvider,
            domainIdentityToDataIdentityMapper,
            departmentDataIdentityToDomainIdentityMapper)
    {
        _departmentDataIdentityToDomainIdentityMapper = departmentDataIdentityToDomainIdentityMapper;
        _employeeDataIdentityToDomainIdentityMapper = employeeDataIdentityToDomainIdentityMapper;
    }

    protected override Unit MapToApiResponse(Unit domainResponse) => domainResponse;

    protected override DepartmentCreationCommand MapToDomainRequest(DepartmentCreationApiCommand apiRequest)
    {
        var tenantId = _tenantIdProvider.GetTenantId();
        var userId = _userIdProvider.GetUserId();

        if (apiRequest.ManagerId == null)
        {
            throw new InvalidOperationException("Manager ID is required when creating a department");
        }

        return new DepartmentCreationCommand(
            tenantId,
            userId,
            apiRequest.Name ?? string.Empty,
            Optional(apiRequest.ParentDepartmentId).Map(_departmentDataIdentityToDomainIdentityMapper.Map),
            _employeeDataIdentityToDomainIdentityMapper.Map(apiRequest.ManagerId.Value));
    }
}
