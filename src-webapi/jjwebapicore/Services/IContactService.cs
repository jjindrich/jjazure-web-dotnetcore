using jjwebapicore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jjwebapicore.Services
{
    public interface IContactService
    {
        List<Contact> GetAll();
    }
}
