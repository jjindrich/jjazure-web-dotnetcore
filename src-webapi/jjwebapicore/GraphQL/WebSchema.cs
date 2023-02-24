using GraphQL.Types;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace jjwebapicore.GraphQL.Types
{
    public class WebSchema : Schema
    {
        public WebSchema(IServiceProvider services) : base(services)
        {
            Query = services.GetRequiredService<WebQuery>();
        }
    }
}
