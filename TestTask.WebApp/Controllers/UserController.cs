using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestTask.Models;
using TestTask.Repositories;
using TestTask.Requests;
using TestTask.Services;

namespace TestTask.Controllers
{
    [Authorize(AuthenticationSchemes ="Cookies", Roles ="User")]            
    [Route("User")]
    public class UserController: Controller
    {
        private readonly UnitOfWork unitOfWork;
        private readonly Generator generator;

        public UserController(UnitOfWork unitOfWork,  Generator generator) 
        {
            this.unitOfWork = unitOfWork;
            this.generator = generator;          
        }

        [HttpGet("item")] //получить товар по ID - Ok
        public async Task<IActionResult> GetItem(Guid guid) 
        {
             return Json(await unitOfWork.Items.GetAsync(guid));
        }

        [AllowAnonymous]
        [HttpGet("allitems")] //получить все товары  - Ok
        public async Task<IActionResult> GetAllItems()
        {
            return Json(await unitOfWork.Items.GetAllAsync());
        }

        [HttpGet("allorders")] //Клиент получает все свои заказы -Ok
        public async Task<IActionResult> GetAllOrders() 
        {
            return Json(await unitOfWork.Orders.GetOrdersByCustomer((
                await unitOfWork.Customers.GetCurrentCustomer(HttpContext)).Id
                    )); 
        }

        [HttpGet("ordersbystatus")] //Получение заказов по статусу -Ok
        public async Task<IActionResult> GetOrdersByStatus([FromBody] GetOrderByStatus request)
        {

            if (ModelState.IsValid)
            {
                List<Order> filteredorders = await unitOfWork.Orders.GetOrdersByStatus(request.orderStatus);
                return Json(filteredorders);
            }
            else
            {
                return BadRequest("Incorrect input");
            }
          
        }

        [HttpGet("shopcart")] //получение элементов корзины  -Ok
        public async Task<IActionResult> GetShopCart()
        { 
            Order shopCart = await unitOfWork.Orders.GetShopCart(
                (await unitOfWork.Customers.GetCurrentCustomer(HttpContext)).Id);
            return Json(await unitOfWork.OrderElements.GetShopCartElements(shopCart.OrderId));     
        }
   
        [HttpPost("addtocart")] //Добавить товары в корзину  - Ok            
        public async Task<IActionResult> AddItemToShopCart([FromBody] AddItemsToShopCart requestmodel)
        {
            if (ModelState.IsValid)
            {
                var customer = await unitOfWork.Customers.GetCurrentCustomer(HttpContext);//поиск текущего пользователя
                var item = await unitOfWork.Items.GetAsync(requestmodel.ItemId); // поиск товара по Id
                if (await unitOfWork.Orders.GetShopCart(customer.Id) == null) //случай если несделанный заказ не существует
                {                    
                    Order order = new Order()                   
                    {    
                        OrderId = Guid.NewGuid(),
                        CustomerId = customer.Id,                                       
                        Customer = customer
                    };
                    await unitOfWork.Orders.CreateAsync(order);
                    
                        OrderElement orderElement = new OrderElement(requestmodel.ItemId, requestmodel.ItemCount)
                        {
                            OrderElementId = Guid.NewGuid(),
                            OrderId = order.OrderId,
                            ItemPrice = item.Price,
                            Order = order
                        };
                        await unitOfWork.OrderElements.CreateAsync(orderElement);
                        order.OrderElements.Add(orderElement);                                      
                }                                               
               else  //случай если несделанный заказ существует
                {                                               
                    var order = await unitOfWork.Orders.GetShopCart(customer.Id); //поиск пустого заказа пользователя
                    if (await unitOfWork.OrderElements.GetElementByItem(requestmodel.ItemId) == null) //случай если товара ещё такого нет
                    {
                        OrderElement orderElement = new OrderElement(requestmodel.ItemId, requestmodel.ItemCount)
                        {
                            OrderElementId = Guid.NewGuid(),
                            OrderId = order.OrderId,
                            ItemPrice = item.Price,
                            Order = order
                        };
                        await unitOfWork.OrderElements.CreateAsync(orderElement);
                        order.OrderElements.Add(orderElement);
                    }
                    else //слчай если товар уже такой есть
                    {
                        OrderElement el = await unitOfWork.OrderElements.GetElementByItem(requestmodel.ItemId);
                        el.ItemCount += requestmodel.ItemCount;
                        await unitOfWork.OrderElements.Update(el);
                    }
                }
                await unitOfWork.SaveAsync();
                return Ok();
            }
            else
            {
                return (BadRequest("Incorrect request")); 
            }
        }

        [HttpPost("delfromcart")] //Удалить элементы из корзины  - Ok
        public async Task<IActionResult> DelItemFromShopCart([FromBody] DelItemsFromShopCart requestmodel)
        {
            if (ModelState.IsValid)
            {
                var customer = await unitOfWork.Customers.GetCurrentCustomer(HttpContext);//поиск текущего пользователя
                var item = await unitOfWork.Items.GetAsync(requestmodel.ItemId); // поиск товара по Id
                if (await unitOfWork.Orders.GetShopCart(customer.Id) == null)
                {
                    return BadRequest("Your shopcart is empty!");
                }
                else
                {
                    var OrderElement = await unitOfWork.OrderElements.GetElementByItem(requestmodel.ItemId);
                    await unitOfWork.OrderElements.DeleteAsync(OrderElement.OrderElementId);

                }
                return Ok();
            }
            else
            {
                return (BadRequest("Incorrect request")); 
            }
        }

        [HttpPost("makeorder")] //Сделать заказ - Ok
        public async Task MakeOrder() 
        {
            var customer = await unitOfWork.Customers.GetCurrentCustomer(HttpContext);            
            Random rnd = new Random();
            var order = await unitOfWork.Orders.GetShopCart(customer.Id);
            order.OrderStatus = OrderStatus.New;
            order.OrderDate = DateTime.Now;               
            order.OrderNumber = rnd.Next(1000000, 10000000);
            order.OrderElements = await unitOfWork.OrderElements.GetShopCartElements(order.OrderId);
            await unitOfWork.Orders.Update(order);
        }

        [HttpPost("deleteorder")] //Удалить заказ - Ok
        public async Task DeleteOrder(Guid orderid)
        {
            var order = await unitOfWork.Orders.GetAsync(orderid);
            if (order.OrderStatus == OrderStatus.New)
            {
                await unitOfWork.Orders.DeleteAsync(orderid);
            }
            else
            {
                BadRequest("Your order is processing or finished");
            }
        }

    }
}
