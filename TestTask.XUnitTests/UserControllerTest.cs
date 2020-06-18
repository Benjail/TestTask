using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using TestTask.Controllers;
using TestTask.Models;
using TestTask.Requests;
using TestTask.WebApp.Repositories.Interfaces;
using Xunit;

namespace TestTask.XUnitTests
{
    public class UserControllerTest
    {
        [Fact]
        public async void GetAllItemsOk()
        { //Arrange
            var unitofworkMock = GetUnitOfWorkMock();
            unitofworkMock.Setup(unit => unit.Items.GetAllAsync())
                .ReturnsAsync(GetItemsMock());
            UserController userController = new UserController(
                unitofworkMock.Object);

            //Act
            var result = await userController.GetAllItems() as JsonResult;

            //Assert 
            var itemslist = Assert.IsType<List<Item>>(result.Value);
            Assert.Equal(itemslist.Count, 4);
        }

        [Fact]
        public async void GetItemOk()
        {
            //Arrange
            Item excpected = new Item()
            {
                Name = "Iphone 7",
                Category = "Телефоны",
                Price = 45000,
                Code = "111-222-333-444",
                ItemId = Guid.Parse("c32399aa-b1de-418d-2c04-08d7e5f21232")
            };
            var unitOfWorkMock = GetUnitOfWorkMock();
            unitOfWorkMock.Setup(u => u.Items.GetAsync(excpected.ItemId))
                .ReturnsAsync(GetItemsMock().Find(item => item.ItemId == excpected.ItemId));
            UserController userController = new UserController(unitOfWorkMock.Object);

            //Act
            var result = await userController.GetItem(excpected.ItemId) as JsonResult;
            var item = Assert.IsType<Item>(result.Value);
            //Arrange
            Assert.Equal(item.Name, excpected.Name);
            Assert.Equal(excpected.Category, item.Category);        
            Assert.Equal(excpected.Code, item.Code);        
            Assert.Equal(excpected.Price, item.Price);      

        }

        [Fact]
        public async void GetItemErrorNotFound()
        {
            //Arrange
            Item excpected = new Item()
            {
                Name = "Iphone 7",
                Category = "Телефоны",
                Price = 45000,
                Code = "111-222-333-444",
                ItemId = Guid.Parse("c32399aa-b1de-418d-2c04-08d7e5f21233")
            };
            var unitOfWorkMock = GetUnitOfWorkMock();
            unitOfWorkMock.Setup(u => u.Items.GetAsync(excpected.ItemId))
                .ReturnsAsync(GetItemsMock().Find(item => item.ItemId == excpected.ItemId));
            UserController userController = new UserController(unitOfWorkMock.Object);

            //Act
            var result = await userController.GetItem(excpected.ItemId) as NotFoundObjectResult;
            var errorresult = Assert.IsType<string>(result.Value);
            //Arrange
            Assert.Equal(errorresult, "Item is not found");
        }

        [Fact]
        public async void GetAllOrdersOk()
        {
            //Arrange
            var customer = GetCurrentCustomersMock();

            var unitofworkMock = GetUnitOfWorkMock();
            unitofworkMock.Setup(unit => unit.Orders.GetAsync(It.IsAny<Customer>()))
                .ReturnsAsync(GetOrdersMock().FindAll(o=>o.CustomerId == customer.Id));  
            unitofworkMock.Setup(unit=>unit.Customers.GetCurrentCustomer(It.IsAny<HttpContext>()))
               .ReturnsAsync(GetCurrentCustomersMock());               

            UserController userController = new UserController( 
                unitofworkMock.Object);

            //Act
           var result = await userController.GetAllOrders() as JsonResult; 

            //Assert 
            var itemslist = Assert.IsType<List<Order>>(result.Value); 
            Assert.Equal(itemslist.Count, 2); 
        }

        [Fact]
        public async void GetOrderOk()
        {
            //Arrange
            Guid OrderId = Guid.Parse("0179119c-5779-4c55-9ee2-972a815b49ad");
            var unitofworkMock = GetUnitOfWorkMock();
            unitofworkMock.Setup(u => u.Orders.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(GetOrdersMock().Find(o => o.OrderId == OrderId));
            UserController userController = new UserController(
                unitofworkMock.Object);
            //Act
            var result = await userController.GetOrder(OrderId) as JsonResult;
            //Assert
            var order = Assert.IsType<Order>(result.Value);
            Assert.Equal(order.OrderNumber,123);
        }

        [Fact]
        public async void GetOrderErrorNotFound()
        {
            //Arrange
            Guid OrderId = Guid.Parse("00000000-5779-4c55-9ee2-972a815b49a1");
            var unitofworkMock = GetUnitOfWorkMock();
            unitofworkMock.Setup(u => u.Orders.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(GetOrdersMock().Find(o => o.OrderId == OrderId));
            UserController userController = new UserController(
                unitofworkMock.Object);
            //Act
            var result = await userController.GetOrder(OrderId) as NotFoundObjectResult;
            //Assert
            var ordererror = Assert.IsType<string>(result.Value);
            Assert.Equal(ordererror, "Not found the order");
        }

        [Fact]
        public async void GetOrdersByStatusOk()
        {
            //Arrange
            GetOrderByStatus request = new GetOrderByStatus()
            {
                orderStatus = OrderStatus.New
            };
            var unitofworkMock = GetUnitOfWorkMock();
            unitofworkMock.Setup(u => u.Orders.GetAsync(It.IsAny<OrderStatus>()))
                .ReturnsAsync(GetOrdersMock().FindAll(o => o.OrderStatus == request.orderStatus));
            UserController userController = new UserController(
                unitofworkMock.Object);
            //Act
            var result = await userController.GetOrdersByStatus(request) as JsonResult;
            //Assert
            var orders = Assert.IsType<List<Order>>(result.Value);
            Assert.Equal(orders.Count, 1);
        }

        [Fact]
        public async void GetOrdersByStatusNotFound()
        {
            //Arrange
            GetOrderByStatus request = new GetOrderByStatus()
            {
                orderStatus = OrderStatus.Finished
            };
            var unitofworkMock = GetUnitOfWorkMock();
            unitofworkMock.Setup(u => u.Orders.GetAsync(It.IsAny<OrderStatus>()))
                .ReturnsAsync(GetOrdersMock().FindAll(o => o.OrderStatus == request.orderStatus));
            UserController userController = new UserController(
                unitofworkMock.Object);

            //Act
            var result = await userController.GetOrdersByStatus(request) as NotFoundObjectResult;  

            //Assert
            var orderserror = Assert.IsType<string>(result.Value);          
            Assert.Equal(orderserror, "Order is not found");                 
        }

        [Fact] 
        public async void DeleteorderOk()
        {
            //Arange 
            Guid OrderId = Guid.Parse("0179119c-5779-4c55-9ee2-972a815b49ad");
            var unitofworkMock = GetUnitOfWorkMock();
            unitofworkMock.Setup(u => u.Orders.DeleteAsync(It.IsAny<Guid>()));
            unitofworkMock.Setup(u => u.Orders.GetAsync(It.IsAny<Guid>()))
               .ReturnsAsync(GetOrdersMock().Find(o => o.OrderId == OrderId));

            UserController userController = new UserController(
                unitofworkMock.Object);

            //Act
            await userController.DeleteOrder(OrderId);
            //Assert
            unitofworkMock.Verify(o => o.Orders.GetAsync(It.IsAny<Guid>()));

            unitofworkMock.Verify(o => o.Orders.DeleteAsync(It.IsAny<Guid>()));
        }

        [Fact]
        public async void DeleteorderErrorNotFound()
        {
            //Arange 
            Guid OrderId = Guid.Parse("00000000-5779-4c55-9ee2-972a815b49ad");
            var unitofworkMock = GetUnitOfWorkMock();
            unitofworkMock.Setup(u => u.Orders.DeleteAsync(It.IsAny<Guid>()));
            unitofworkMock.Setup(u => u.Orders.GetAsync(It.IsAny<Guid>()))
               .ReturnsAsync(GetOrdersMock().Find(o => o.OrderId == OrderId));
            UserController userController = new UserController(
                unitofworkMock.Object);

            //Act
            var result = await userController.DeleteOrder(OrderId) as NotFoundObjectResult;
            var errorresult = Assert.IsType<string>(result.Value);

            //Assert
            Assert.Equal(errorresult, "Not found the order");
        }

        [Fact]
        public async void DeleteorderErrorIsProcessing()
        {
            //Arange 
            Guid OrderId = Guid.Parse("f6e6bd2a-ddb8-4db3-ba7b-51dd226e7de2");
            var unitofworkMock = GetUnitOfWorkMock();
            unitofworkMock.Setup(u => u.Orders.DeleteAsync(It.IsAny<Guid>()));
            unitofworkMock.Setup(u => u.Orders.GetAsync(It.IsAny<Guid>()))
               .ReturnsAsync(GetOrdersMock().Find(o => o.OrderId == OrderId));
            UserController userController = new UserController(
                unitofworkMock.Object);

            //Act
            var result = await userController.DeleteOrder(OrderId) as BadRequestObjectResult;
            var errorresult = Assert.IsType<string>(result.Value);

            //Assert
            Assert.Equal(errorresult, "Your order is processing or finished");  
        }

        // [Fact]
        public async void GetShopCartOk()//??????????
        {
          //  HttpContext httpContext = new HttpContext();
             
        }
        private List<Item> GetItemsMock()
        {
            return new List<Item>
            {
                new Item { Name="Iphone 7",Category="Телефоны", Price=45000, Code ="111-222-333-444" , ItemId=Guid.Parse("c32399aa-b1de-418d-2c04-08d7e5f21232")},
                new Item { Name="Iphone 8",Category="Телефоны", Price=50000, Code ="111-222-333-444" , ItemId=Guid.Parse("8b0e9dfb-5f86-47ed-2c07-08d7e5f21232")},
                new Item { Name="Iphone 6",Category="Телефоны", Price=20000, Code ="111-222-333-444" , ItemId=Guid.Parse("ba66abd1-9b29-4acf-9441-08d7e6183b89")},
                new Item { Name="Iphone 11",Category="Телефоны", Price=70000, Code ="111-222-333-444", ItemId=Guid.Parse("62142b35-13b8-44fd-af58-08d7e74e3c0c")}
            };
        }
       
        private List<Order> GetOrdersMock()
        {
            return new List<Order>
            {
                new Order{ CustomerId = Guid.Parse("fe3a3c6d-1217-439a-5786-08d7e77b2011"),
                    OrderId = Guid.Parse("0179119c-5779-4c55-9ee2-972a815b49ad"),
                    OrderDate = new DateTime(01, 01, 01, 00, 00, 00),
                    OrderStatus =OrderStatus.New,
                    OrderNumber= 123
                },
                 new Order{ CustomerId = Guid.Parse("fe3a3c6d-1217-439a-5786-08d7e77b2011"),
                    OrderId = Guid.Parse("f6e6bd2a-ddb8-4db3-ba7b-51dd226e7de2"),
                    OrderDate = DateTime.Today,
                    OrderStatus =OrderStatus.InProceed,
                    OrderNumber= 124
                }
            };
        }
        private Customer GetCurrentCustomersMock()
        {
            return new Customer
            {
                Name = "Misha",
                Code = "212-662-333-154",
                Id = Guid.Parse("fe3a3c6d-1217-439a-5786-08d7e77b2011"),
                Email = "miha203@mail.ru",
                Discount = 5
            };          
        }

        private Mock<HttpContext> GetContext()
        {
            return new Mock<HttpContext>();
        }

        private Mock<IUnitOfWork> GetUnitOfWorkMock()
        {
            return new Mock<IUnitOfWork>();
        }
    }
}
