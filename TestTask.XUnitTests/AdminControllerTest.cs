using Xunit;
using TestTask.Models;
using Moq;
using TestTask.WebApp.Repositories.Interfaces;
using System.Collections.Generic;
using System;
using TestTask.Controllers;
using TestTask.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestTask.Requests;

namespace TestTask.XUnitTests
{
    public class AdminControllerTest
    {
        [Fact]
        public async void GetItemOk()
        {
            //Arrange
            Item excpected = new Item()
            { Name = "Iphone 7",
                Category = "Телефоны",
                Price = 45000, Code = "111-222-333-444",
                ItemId = Guid.Parse("c32399aa-b1de-418d-2c04-08d7e5f21232")
            };
            var unitOfWorkMock = GetUnitOfWorkMock();
            unitOfWorkMock.Setup(u => u.Items.GetAsync(excpected.ItemId))
                .ReturnsAsync(GetItemsMock().Find(item => item.ItemId == excpected.ItemId));
            AdminController adminController = new AdminController(GetUserManagerMock<Customer>().Object, unitOfWorkMock.Object);
            
            //Act 
            var result = await adminController.GetItem(excpected.ItemId) as JsonResult;

            //Assert  
            Item actual = Assert.IsType<Item>(result.Value);
            Assert.Equal(excpected.ItemId, actual.ItemId);
            Assert.Equal(excpected.Name, actual.Name);
            Assert.Equal(excpected.Category, actual.Category);
            Assert.Equal(excpected.Code, actual.Code);
            Assert.Equal(excpected.Price, actual.Price);
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
            var unitofworkMock = GetUnitOfWorkMock();
            unitofworkMock.Setup(unit => unit.Items.GetAsync(excpected.ItemId))
                .ReturnsAsync(GetItemsMock().Find(item => item.ItemId == excpected.ItemId));
            AdminController adminController = new AdminController(
                GetUserManagerMock<Customer>().Object,
                unitofworkMock.Object);

            //Act
            var result = await adminController.GetItem(excpected.ItemId) as NotFoundObjectResult;

            //Assert 
            var errorresult = Assert.IsType<string>(result.Value);
            Assert.Equal(errorresult, "Item is not found");
        }

        [Fact]
        public async void GetAllItemsOk()
        { //Arrange
            var unitofworkMock = GetUnitOfWorkMock();
            unitofworkMock.Setup(unit => unit.Items.GetAllAsync())
                .ReturnsAsync(GetItemsMock());
            AdminController adminController = new AdminController(
                GetUserManagerMock<Customer>().Object,
                unitofworkMock.Object);

            //Act
            var result = await adminController.GetAllItems() as JsonResult;

            //Assert 
            var itemslist = Assert.IsType<List<Item>>(result.Value);
            Assert.Equal(itemslist.Count, 4);
        }

        [Fact]
        public async void AddItemOk()
        {
            Item newitem = new Item()
            {
                Name = "Iphone 7",
                Category = "Телефоны",
                Price = 45000,
                Code = "111-222-333-444"
            };

            var unitofworkMock = GetUnitOfWorkMock();
            unitofworkMock.Setup(unit => unit.Items.CreateAsync(It.IsAny<Item>()));

            AdminController adminController = new AdminController(
                GetUserManagerMock<Customer>().Object,
                unitofworkMock.Object);

            AddItemModel model = new AddItemModel()
            {
                Category = newitem.Category,
                Price = newitem.Price,
                Code = newitem.Code,
                Name = newitem.Name
            };

            //Act
            await adminController.AddItem(model);

            //Assert 
            unitofworkMock.Verify(u => u.Items.CreateAsync(It.IsAny<Item>()));
        }

        /*   [Fact]
             public async void AddItemErrorAddItemModel()  
             {
                 Item newitem = new Item()
                 {
                     Name = "Iphone 7",
                     Category = "Телефоны",
                     Price = 45000,
                     Code = "111-222-333-444",
                 };
                 var unitofworkMock = GetUnitOfWorkMock();
                 unitofworkMock.Setup(unit => unit.Items.CreateAsync(It.IsAny<Item>()));

                 AdminController adminController = new AdminController(
                     GetUserManagerMock<Customer>().Object,   
                     unitofworkMock.Object);

                 var model = new AddItemModel();
                 {
                     //No category
                     model.Category = "Телефоны";
                     model.Price = 45000;
                     model.Code = "111-222-333-444";
                     model.Name = "Iphone 7";
                 }

                 //Act
                 var result = await adminController.AddItem(model) as BadRequestObjectResult;

                 //Assert 
                 var errorresult = Assert.IsType<string>(result.Value);
                 Assert.Equal(errorresult, "Request model is invalid");
                 //unitofworkMock.Verify(u => u.Items.CreateAsync(It.IsAny<Item>()));
             }*/ //неполная модель


        [Fact]
        public async void DeleteItemOk()
        {
            //Arrange
            Guid id = Guid.Parse("c32399aa-b1de-418d-2c04-08d7e5f21232");

            var unitofworkMock = GetUnitOfWorkMock();
            unitofworkMock.Setup(unit => unit.Items.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(GetItemsMock().Find(item => item.ItemId == id));
            unitofworkMock.Setup(unit => unit.Items.DeleteAsync(It.IsAny<Guid>()));
            AdminController adminController = new AdminController(
                GetUserManagerMock<Customer>().Object,
                unitofworkMock.Object);

            //Act 
            await adminController.ItemDelete(id);

            //Assert 

            unitofworkMock.Verify(u => u.Items.DeleteAsync(It.IsAny<Guid>()));
        }

        [Fact]
        public async void DeleteItemErrorNotFound()
        {
            //Arrange
            Guid id = Guid.Parse("c32399aa-b1de-418d-2c04-08d7e5f21233");

            var unitofworkMock = GetUnitOfWorkMock();
            unitofworkMock.Setup(unit => unit.Items.GetAsync(It.IsAny<Guid>()))
               .ReturnsAsync(GetItemsMock().Find(item => item.ItemId == id));
            unitofworkMock.Setup(unit => unit.Items.DeleteAsync(It.IsAny<Guid>()));

            AdminController adminController = new AdminController(
              GetUserManagerMock<Customer>().Object,
              unitofworkMock.Object);

            //Act
            var result = await adminController.ItemDelete(id) as NotFoundObjectResult;
            var errorresult = Assert.IsType<string>(result.Value);
            //Assert
            Assert.Equal(errorresult, "Item is not found");
        }

        [Fact]
        public async void UpdateItemOk()
        {
            //Arrange
            EditedItemModel excpected = new EditedItemModel()
            {
                Name = "Iphone 7",
                Category = "Телефоны",
                Price = 45000,
                Code = "111-222-333-444",
                EditedItemId = Guid.Parse("c32399aa-b1de-418d-2c04-08d7e5f21232")
            };

            var unitofworkMock = GetUnitOfWorkMock();
            unitofworkMock.Setup(unit => unit.Items.Update(It.IsAny<Item>()));
            unitofworkMock.Setup(unit => unit.Items.GetAsync(It.IsAny<Guid>()))
              .ReturnsAsync(GetItemsMock().Find(item => item.ItemId == excpected.EditedItemId));

            AdminController adminController = new AdminController(
                GetUserManagerMock<Customer>().Object,
                unitofworkMock.Object);

            //Act
            await adminController.UpdateItem(excpected);
            //Assert
            unitofworkMock.Verify(u => u.Items.Update(It.IsAny<Item>()));

        }

        [Fact]
        public async void UpdateItemErrorNotFound()
        {
            //Arrange
            EditedItemModel excpected = new EditedItemModel()
            {
                Name = "Iphone 7",
                Category = "Телефоны",
                Price = 45000,
                Code = "111-222-333-444",
                EditedItemId = Guid.Parse("c32399aa-b1de-418d-2c04-08d7e5f21233")
            };

            var unitofworkMock = GetUnitOfWorkMock();
            unitofworkMock.Setup(unit => unit.Items.Update(It.IsAny<Item>()));
            unitofworkMock.Setup(unit => unit.Items.GetAsync(It.IsAny<Guid>()))
              .ReturnsAsync(GetItemsMock().Find(item => item.ItemId == excpected.EditedItemId));

            AdminController adminController = new AdminController(
                GetUserManagerMock<Customer>().Object,
                unitofworkMock.Object);

            //Act
            var result = await adminController.UpdateItem(excpected) as NotFoundObjectResult;
            var errorresult = Assert.IsType<string>(result.Value);

            //Assert
            Assert.Equal(errorresult, "Item is not found");
        }

        [Fact]
        public async void GetAllCustomersOk()
        {
            //Arrange
            var unitofworkMock = GetUnitOfWorkMock();
            unitofworkMock.Setup(unit => unit.Customers.GetAllAsync())
                .ReturnsAsync(GetCustomersMock());

            AdminController adminController = new AdminController(
               GetUserManagerMock<Customer>().Object,
               unitofworkMock.Object);

            //Act
            var result = await adminController.GetAllCustomers() as JsonResult;
            var customerlist = Assert.IsType<List<Customer>>(result.Value);
            //Assert
            Assert.Equal(customerlist.Count, 1);
        }

        [Fact]
        public async void GetCustomerOk()
        {
            //Arrange
            Customer expected = new Customer()
            {
                Name = "Misha",
                Code = "212-662-333-154",
                Id = Guid.Parse("fe3a3c6d-1217-439a-5786-08d7e77b2011"),
                Email = "miha203@mail.ru",
                Discount = 5
            };
            var unitofworkMock = GetUnitOfWorkMock();
            unitofworkMock.Setup(unit => unit.Customers.GetAsync(expected.Id))
                .ReturnsAsync(GetCustomersMock().Find(c => c.Id == expected.Id));
            AdminController adminController = new AdminController(
               GetUserManagerMock<Customer>().Object,
               unitofworkMock.Object);

            //Act 
            var result = await adminController.GetCustomer(expected.Id) as JsonResult;

            //Assert  
            var actual = Assert.IsType<Customer>(result.Value);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Code, actual.Code);
            Assert.Equal(expected.Discount, actual.Discount);
        }

        [Fact]
        public async void GetCustomerErrorNotFound()
        {
            //Arrange
            Customer expected = new Customer()
            {
                Name = "Misha",
                Code = "212-662-333-154",
                Id = Guid.Parse("fe3a3c6d-1217-439a-5786-08d7e77b2012"),
                Email = "miha203@mail.ru",
                Discount = 5
            };
            var unitofworkMock = GetUnitOfWorkMock();
            unitofworkMock.Setup(unit => unit.Customers.GetAsync(expected.Id))  
                .ReturnsAsync(GetCustomersMock().Find(c => c.Id == expected.Id));   
            AdminController adminController = new AdminController(  
               GetUserManagerMock<Customer>().Object,  
               unitofworkMock.Object);  

            //Act 
            var result = await adminController.GetCustomer(expected.Id) as NotFoundObjectResult;
            //Assert
            var errorresult = Assert.IsType<string>(result.Value);
            Assert.Equal(errorresult, "Customer not found");                      
        }

        [Fact]
        public async void AddCustomerOk()
        {
            //Arrange
            Customer expected = new Customer()
            {
                Name = "Misha",
                Code = "212-662-333-154",
                Id = Guid.Parse("fe3a3c6d-1217-439a-5786-08d7e77b2011"),
                Email = "miha203@mail.ru",
                Discount = 5,
                Address = "Moscow"
            };
            var unitofworkMock = GetUnitOfWorkMock();
            unitofworkMock.Setup(unit => unit.Customers.GetAsync(expected.Id))
                .ReturnsAsync(GetCustomersMock().Find(c => c.Id == expected.Id));
            unitofworkMock.Setup(unit => unit.Customers.CreateAsync(expected, new Generator().GetPass()));
            AdminController adminController = new AdminController(
               GetUserManagerMock<Customer>().Object,
               unitofworkMock.Object);

            //Act
            AddUserModel model = new AddUserModel()
            {
                Name = expected.Name,
                Email = expected.Email,
                Code = expected.Code,
                Address = expected.Address,
                Discount = expected.Discount
            };
            await adminController.AddCustomer(model);

            //Assert
            unitofworkMock.Verify(u => u.Customers.CreateAsync(It.IsAny<Customer>(), It.IsAny<string>()));
        }
        [Fact]
        public async void AddCustomerErrorExist()
        {
            Customer expected = new Customer()
            {
                Name = "Misha",
                Code = "212-662-333-154",
                Id = Guid.Parse("fe3a3c6d-1217-439a-5786-08d7e77b2011"),
                Email = "miha203@mail.ru",
                Discount = 5,
                Address = "Moscow"
            };
            var unitofworkMock = GetUnitOfWorkMock();
            unitofworkMock.Setup(unit => unit.Customers.GetAsync(expected.Email))
                .ReturnsAsync(GetCustomersMock().Find(c => c.Email == expected.Email));
            unitofworkMock.Setup(unit => unit.Customers.CreateAsync(expected, new Generator().GetPass()));
            AdminController adminController = new AdminController(
               GetUserManagerMock<Customer>().Object,
               unitofworkMock.Object);

            //Act
            AddUserModel model = new AddUserModel()
            {
                Name = expected.Name,
                Email = expected.Email,
                Code = expected.Code,
                Address = expected.Address,
                Discount = expected.Discount
            };
            var result = await adminController.AddCustomer(model) as BadRequestObjectResult;
            var errorresult = Assert.IsType<string>(result.Value);
            //Assert 
            Assert.Equal(errorresult, "This user already exists");
        }

        [Fact]
        public async void UpdateCustomerOk()
        {
            EditedUserModel model = new EditedUserModel()
            {
                Name = "Misha",
                Code = "212-662-333-154",
                Email = "miha203@mail.ru",
                Discount = 5,
                Address = "Moscow",
                EditedUserId = Guid.Parse("fe3a3c6d-1217-439a-5786-08d7e77b2011")
            };
            var unitOfWorkMock = GetUnitOfWorkMock();
            unitOfWorkMock.Setup(u => u.Customers.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(GetCustomersMock().Find(c => c.Id == model.EditedUserId));
            unitOfWorkMock.Setup(u => u.Customers.Update(It.IsAny<Customer>()));

            AdminController adminController = new AdminController(
                GetUserManagerMock<Customer>().Object,
                unitOfWorkMock.Object);

            //Act

            await adminController.UpdateCustomer(model);
            //Assert
            unitOfWorkMock.Verify(u => u.Customers.Update(It.IsAny<Customer>()));
        }
        [Fact]
        public async void UpdateCustomerErrorNotFound()
        {
            EditedUserModel model = new EditedUserModel()
            {
                Name = "Misha",
                Code = "212-662-333-154",
                Email = "miha203@mail.ru",
                Discount = 5,
                Address = "Moscow",
                EditedUserId = Guid.Parse("fe3a3c6d-1217-439a-5786-08d7e77b2012")
            };
            var unitOfWorkMock = GetUnitOfWorkMock();
            unitOfWorkMock.Setup(u => u.Customers.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(GetCustomersMock().Find(c => c.Id == model.EditedUserId));
            unitOfWorkMock.Setup(u => u.Customers.Update(It.IsAny<Customer>()));

            AdminController adminController = new AdminController(
                GetUserManagerMock<Customer>().Object,
                unitOfWorkMock.Object);

            //Act

            var result = await adminController.UpdateCustomer(model) as NotFoundObjectResult;
            //Assert
            var errorresult = Assert.IsType<string>(result.Value);
            Assert.Equal(errorresult, "Customer not found");
        }
        [Fact]
        public async void DeleteCustomerOk()
        {
            //  Arrange
            Guid Id = Guid.Parse("fe3a3c6d-1217-439a-5786-08d7e77b2011");
            var unitOfWorkMock = GetUnitOfWorkMock();
            unitOfWorkMock.Setup(u => u.Customers.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(GetCustomersMock().Find(c => c.Id == Id));
            unitOfWorkMock.Setup(u => u.Customers.DeleteAsync(It.IsAny<Customer>()));

            AdminController adminController = new AdminController(
                GetUserManagerMock<Customer>().Object,
                unitOfWorkMock.Object);
            //Act 
            await adminController.DeleteCustomer(Id);
            //Assert
            unitOfWorkMock.Verify(u => u.Customers.DeleteAsync(It.IsAny<Customer>()));

        }

        [Fact]
        public async void DeleteCustomerErrorNotFound()
        {
            //  Arrange
            Guid Id = Guid.Parse("fe3a3c6d-1217-439a-5786-08d7e77b2012");
            var unitOfWorkMock = GetUnitOfWorkMock();
            unitOfWorkMock.Setup(u => u.Customers.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(GetCustomersMock().Find(c => c.Id == Id));
            unitOfWorkMock.Setup(u => u.Customers.DeleteAsync(It.IsAny<Customer>()));

            AdminController adminController = new AdminController(
                GetUserManagerMock<Customer>().Object,
                unitOfWorkMock.Object);
            //Act 
            var result = await adminController.DeleteCustomer(Id) as NotFoundObjectResult;
            //Assert 
            var errorresult = Assert.IsType<string>(result.Value);
            Assert.Equal(errorresult, "Customer not found");

        }

        [Fact]
        public async void ConfirmOrderOk()
        {
            //Arrange
            ConfirmOderModel confirmOderModel = new ConfirmOderModel()
            {
                DeliveryDate = DateTime.Today,
                OrderId= Guid.Parse("0179119c-5779-4c55-9ee2-972a815b49ad")
            };
            var unitOfWorkMock = GetUnitOfWorkMock();
            unitOfWorkMock.Setup(u => u.Orders.GetAsync(It.IsAny<Guid>()))
               .ReturnsAsync(GetOrdersMock().Find(o => o.OrderId ==confirmOderModel.OrderId));

            AdminController adminController = new AdminController(
               GetUserManagerMock<Customer>().Object,
               unitOfWorkMock.Object);

            //Act
            await adminController.ConfirmOrder(confirmOderModel);

            //Assert
            unitOfWorkMock.Verify(u => u.Orders.GetAsync(It.IsAny<Guid>())); 
        }
        [Fact]
        public async void ConfirmOrderErrorNotFound()
        {
            //Arrange
            ConfirmOderModel confirmOderModel = new ConfirmOderModel()
            {
                DeliveryDate = DateTime.Today,
                OrderId = Guid.Parse("0179119c-5779-4c55-9ee2-972a815b49a0")
            };
            var unitOfWorkMock = GetUnitOfWorkMock();
            unitOfWorkMock.Setup(u => u.Orders.GetAsync(It.IsAny<Guid>()))
               .ReturnsAsync(GetOrdersMock().Find(o => o.OrderId == confirmOderModel.OrderId));

            AdminController adminController = new AdminController(
               GetUserManagerMock<Customer>().Object,
               unitOfWorkMock.Object);

            //Act
            var result = await adminController.ConfirmOrder(confirmOderModel) as NotFoundObjectResult;
            var errorresult = Assert.IsType<string>(result.Value);

            //Assert
            Assert.Equal(errorresult, "Order not found");                           
        }
        [Fact]
        public async void CloseOrderOk()
        {
            //Arrange
           Guid guid = Guid.Parse("0179119c-5779-4c55-9ee2-972a815b49ad");
           var unitOfWorkMock = GetUnitOfWorkMock();
           unitOfWorkMock.Setup(u => u.Orders.GetAsync(It.IsAny<Guid>()))
              .ReturnsAsync(GetOrdersMock().Find(o => o.OrderId == guid));
            AdminController adminController = new AdminController(
               GetUserManagerMock<Customer>().Object,
               unitOfWorkMock.Object);
            //Act
            await adminController.CloseOrder(guid);
            //Assert
            unitOfWorkMock.Verify(u => u.Orders.GetAsync(It.IsAny<Guid>()));

        }
        [Fact]
        public async void CloseOrderErrorNotFound()
        {
            //Arrange
            Guid guid = Guid.Parse("00000000-5779-4c55-9ee2-972a815b49a0");
            var unitOfWorkMock = GetUnitOfWorkMock();
            unitOfWorkMock.Setup(u => u.Orders.GetAsync(It.IsAny<Guid>()))
               .ReturnsAsync(GetOrdersMock().Find(o => o.OrderId == guid));
            AdminController adminController = new AdminController(
               GetUserManagerMock<Customer>().Object,
               unitOfWorkMock.Object);
            //Act
            var result = await adminController.CloseOrder(guid) as NotFoundObjectResult;
            //Assert 
            var errorresult = Assert.IsType<string>(result.Value);
            Assert.Equal(errorresult, "Order not found");
        }
        private List<Order> GetOrdersMock()
        {
            return new List<Order>  
                {   
                    new Order { CustomerId=Guid.Parse("faf56560-f8b3-4789-abf7-08d7ecee2773"),  
                        OrderId=Guid.Parse("0179119c-5779-4c55-9ee2-972a815b49ad"),     
                        OrderStatus=OrderStatus.New,    
                        OrderNumber = 0  }      
                };  
        }   
        private List<Item> GetItemsMock()                              
        { 
            return new List<Item> 
            {
                new Item { Name="Iphone 7",Category="Телефоны", Price=45000, Code ="111-222-333-444" , ItemId=Guid.Parse("c32399aa-b1de-418d-2c04-08d7e5f21232")},
                new Item { Name="Iphone 8",Category="Телефоны", Price=50000, Code ="111-222-333-444" , ItemId=Guid.Parse("8b0e9dfb-5f86-47ed-2c07-08d7e5f21232")},
                new Item { Name="Iphone 6",Category="Телефоны", Price=20000, Code ="111-222-333-444" , ItemId=Guid.Parse("ba66abd1-9b29-4acf-9441-08d7e6183b89")},
                new Item { Name="Iphone 11",Category="Телефоны",Price=70000, Code ="111-222-333-444" , ItemId=Guid.Parse("62142b35-13b8-44fd-af58-08d7e74e3c0c")}
            };
        }
        private List<Customer> GetCustomersMock()
        {
            return new List<Customer>
            {
                new Customer{ Name ="Misha",
                    Code="212-662-333-154",
                    Id=Guid.Parse("fe3a3c6d-1217-439a-5786-08d7e77b2011"),
                    Email="miha203@mail.ru",
                    Discount = 5}
            };
        }
        private Mock<UserManager<TIDentityUser>> GetUserManagerMock<TIDentityUser>() where TIDentityUser : IdentityUser<Guid>
        {
            return new Mock<UserManager<TIDentityUser>> (
            new Mock<IUserStore<TIDentityUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<TIDentityUser>>().Object,
            new IUserValidator<TIDentityUser>[0],
            new IPasswordValidator<TIDentityUser>[0],
            new Mock<ILookupNormalizer>().Object, 
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<TIDentityUser>>>().Object
            );
        }
        private Mock<IUnitOfWork> GetUnitOfWorkMock() 
        {
            return new Mock<IUnitOfWork>(); 
        }
    } 
}
