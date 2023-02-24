using jjwebapicore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jjwebapicore.Services
{
    public class ContactService : IContactService
    {
        private readonly ContactsContext _context;

        public ContactService(ContactsContext context)
        {
            _context = context;
        }
        public List<Contact> GetAll()
        {
            return _context.Contact.ToList();
        }
    }
}
