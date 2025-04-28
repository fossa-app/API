namespace Fossa.API.Web.ApiModels;

public record DepartmentRetrievalModel(
    long Id,
    string Name,
    long? ParentDepartmentId,
    long ManagerId);
