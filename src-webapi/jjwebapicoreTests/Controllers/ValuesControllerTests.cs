using Microsoft.VisualStudio.TestTools.UnitTesting;
using jjwebapicore.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace jjwebapicore.Controllers.Tests
{
    [TestClass()]
    public class ValuesControllerTests
    {
        ValuesController _controller;

        public ValuesControllerTests()
        {
            _controller = new ValuesController();
        }

        [TestMethod()]
        public void GetTest()
        {
            var result = _controller.Get();
            Assert.IsNotNull(result);

            var content = result.Value.ToList();
            Assert.IsInstanceOfType(content, typeof(IEnumerable<string>));
            Assert.AreEqual(2, content.Count);
        }

        [TestMethod()]
        public void GetTestById()
        {
            var result = _controller.Get(1);
            Assert.IsNotNull(result);

            var content = result.Value;
            Assert.IsInstanceOfType(content, typeof(string));
            Assert.AreEqual("value", content);
        }
    }
}