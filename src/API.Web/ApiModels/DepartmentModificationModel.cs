namespace Fossa.API.Web.ApiModels;

public record DepartmentModificationModel(
    string? Name,
    long? ParentDepartmentId,
    long? ManagerId);
