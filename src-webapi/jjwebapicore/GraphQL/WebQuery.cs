using GraphQL.Types;
using GraphQL;
using GraphQL.MicrosoftDI;
using Microsoft.EntityFrameworkCore;

using jjwebapicore.Models;
using jjwebapicore.Services;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace jjwebapicore.GraphQL
{
    public class WebQuery : ObjectGraphType<object>
    {
        public WebQuery()
        {
            Name = "jjwebapi";

            Field<ListGraphType<ContactType>>(
                "contacts")
                .Resolve(context =>
                {
                    using var scope = context.RequestServices.CreateScope();
                    var services = scope.ServiceProvider;
                    return services.GetRequiredService<IContactService>().GetAll();
                }
                );            
        }
    }
}
