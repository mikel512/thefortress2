using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using DataAccessLibrary.Logic;
using DataAccessLibrary.Models;
using DataAccessLibrary.SqlDataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using TheFortress.Controllers;
using Xunit;

namespace TheFortress.Tests
{
    public class ControllerUnitTest
    {
        [Fact]
        public void Concerts_ReturnsAViewResult()
        {
            // Arrange
            var mockDbContext = new Mock<IDbAccessLogic>();
            mockDbContext.Setup(repo => repo.ApprovedConcertsByMonth())
                .Returns(GetConcerts());
            var controller = new HomeController(mockDbContext.Object);

            // Act
            var result = controller.Concerts();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Dictionary<int, List<LocalConcert>>>(
                viewResult.ViewData.Model);
            Assert.Equal(2, model.Count);
        }

        private Dictionary<int, List<LocalConcert>> GetConcerts()
        {
            var concerts = new Dictionary<int, List<LocalConcert>>();
            concerts[1] = new List<LocalConcert>();
            concerts[2] = new List<LocalConcert>();

            concerts[1].Add(new LocalConcert()
            {
                Artists = "whoo",
                EventConcertId = 1,
                VenueName = "whooo",
            });
            concerts[2].Add(new LocalConcert()
            {
                Artists = "whoo",
                EventConcertId = 1,
                VenueName = "whooo",
            });
            return concerts;
        }

        [Fact]
        public void MessageAdminPost_ReturnsOkResult()
        {
            var mockDbContext = new Mock<IDbAccessLogic>();
            var mockMsgToAdmin = new MessageModel();
            mockDbContext.Setup(repo => repo.AddAdminMessage(mockMsgToAdmin))
                .Returns(1);
            var controller = new HomeController(mockDbContext.Object);

            var result = controller.MessageAdmin(mockMsgToAdmin);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void MessageAdminPost_ReturnsBadRequestResult()
        {
            var mockDbContext = new Mock<IDbAccessLogic>();
            var mockMsgToAdmin = new MessageModel();
            mockDbContext.Setup(repo => repo.AddAdminMessage(mockMsgToAdmin))
                .Returns(1);
            var controller = new HomeController(mockDbContext.Object);
            controller.ModelState.AddModelError("SessionName", "Required");

            var result = controller.MessageAdmin(mockMsgToAdmin);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void AddCommentPost_ReturnsJson()
        {
            // Arrange 
            var mockComment = new CommentModel();
            var mockDbContext = new Mock<IDbAccessLogic>();
            mockDbContext.Setup(
                    repo => repo.AddComment(mockComment))
                .Returns(1);
            var controller = new HomeController(mockDbContext.Object);
            
            // Arrange mock identity
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() {User = user}
            };
            
            // Act
            var result = controller.AddComment(mockComment);

            // Assert
            Assert.IsType<JsonResult>(result);
            
        }
        
        [Fact]
        public void AddCommentPost_ReturnsBadRequest()
        {
            // Arrange 
            var mockComment = new CommentModel();
            var mockDbContext = new Mock<IDbAccessLogic>();
            mockDbContext.Setup(
                    repo => repo.AddComment(mockComment))
                .Returns(1);
            var controller = new HomeController(mockDbContext.Object);
            
            // Arrange mock identity
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() {User = user}
            };
            
            // Add error
            controller.ModelState.AddModelError("SessionName", "Required");
            
            // Act
            var result = controller.AddComment(mockComment);

            // Assert
            Assert.IsType<BadRequestResult>(result);
            
        }
    }
}