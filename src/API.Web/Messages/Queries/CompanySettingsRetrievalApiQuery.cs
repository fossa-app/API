﻿using Fossa.API.Core.Messages.Queries;
using Fossa.API.Web.ApiModels;

namespace Fossa.API.Web.Messages.Queries;

public record CompanySettingsRetrievalApiQuery() : IQuery<CompanySettingsRetrievalModel>;
