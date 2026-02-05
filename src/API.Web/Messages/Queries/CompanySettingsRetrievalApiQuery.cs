using Fossa.API.Web.ApiModels;
using TIKSN.Integration.Messages.Queries;

namespace Fossa.API.Web.Messages.Queries;

public record CompanySettingsRetrievalApiQuery() : IQuery<CompanySettingsRetrievalModel>;
