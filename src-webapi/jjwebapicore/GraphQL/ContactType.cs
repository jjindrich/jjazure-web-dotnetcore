using GraphQL.Types;
using jjwebapicore.Models;
using YamlDotNet.Core.Tokens;

namespace jjwebapicore.GraphQL
{
    public sealed class ContactType: ObjectGraphType<Contact>
    {
        public ContactType()
        {
            Name = nameof(Contact);
            Description = "A contact in the collection";

            Field(m => m.ContactId).Description("Identifier of the contact");
            Field(m => m.FullName).Description("FullName of the contact");
        }
    }
}
