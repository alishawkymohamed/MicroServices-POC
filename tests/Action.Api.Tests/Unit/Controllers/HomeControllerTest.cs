using System;
using Actio.Api.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Action.Api.Tests.Unit.Controllers
{
    public class HomeControllerTest
    {
        [Fact]
        public void home_controller_get_test_should_return_string_content()
        {
            var controller = new HomeController();
            var result = controller.Get();
            var contentResult = result as ContentResult;
            // contentResult.Should().BeOfType(typeof(ContentResult));
            contentResult.Should().NotBeNull();
            contentResult.Content.Should().BeEquivalentTo("Welcome To My Actio API");
        }
    }
}