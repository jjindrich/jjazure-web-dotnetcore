using Microsoft.VisualStudio.TestTools.UnitTesting;
using jjwebapicore.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using SQLitePCL;
using jjwebapicore.Models;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace jjwebapicore.Controllers.Tests
{
    [TestClass()]
    public class ContactsControllerTests
    {
        ContactsController _controller;
        ContactsContext _context;

        public ContactsControllerTests()
        {
        }

        private async Task<ContactsContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ContactsContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new ContactsContext(options);
            databaseContext.Database.EnsureCreated();

            for (int i = 1; i <= 10; i++)
            {
                databaseContext.Contact.Add(new Contact()
                {
                    ContactId = i,
                    FullName = "fullname"

                });
                await databaseContext.SaveChangesAsync();
            }

            return databaseContext;
        }

        [TestMethod()]
        public async Task GetContactTest()
        {
            _context = await GetDatabaseContext();
            _controller = new ContactsController(_context);

            var result = _controller.GetContact().Result;
            Assert.IsNotNull(result);

            var content = result.Value.ToList();
            Assert.IsInstanceOfType(content, typeof(IEnumerable<Contact>));
            Assert.AreEqual(10, content.Count);
        }

        [TestMethod()]
        public async Task GetContactTestById()
        {
            _context = await GetDatabaseContext();
            _controller = new ContactsController(_context);

            var result = _controller.GetContact(1).Result;
            Assert.IsNotNull(result);

            var content = result.Value;
            Assert.IsInstanceOfType(content, typeof(Contact));
            Assert.AreEqual(1, content.ContactId);
            Assert.AreEqual("fullname", content.FullName);
        }
    }
}