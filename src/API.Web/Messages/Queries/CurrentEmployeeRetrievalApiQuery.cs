using Fossa.Bridge.Models.ApiModels;
using TIKSN.Integration.Messages.Queries;

namespace Fossa.API.Web.Messages.Queries;

public record CurrentEmployeeRetrievalApiQuery() : IQuery<EmployeeRetrievalModel>;
