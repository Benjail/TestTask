using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestTask.Models;
using TestTask.Requests;
using TestTask.Services;
using TestTask.WebApp.Repositories.Interfaces;
using TestTask.WebApp.Services;

namespace TestTask.Controllers                                  
{
    [Authorize(AuthenticationSchemes = "Cookies", Roles = "User")]
    [Route("User")]
    public class UserController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("item")] //получить товар по ID - Ok
        public async Task<IActionResult> GetItem(Guid guid)
        {
            if (await unitOfWork.Items.GetAsync(guid) != null)
            {
                return Json(await unitOfWork.Items.GetAsync(guid));
            }
            else
            {
                return NotFound("Item is not found");
            }
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
            return Json(await unitOfWork.Orders.GetAsync((
                await unitOfWork.Customers.GetCurrentCustomer(HttpContext))
                    ));
        }

        [HttpGet("ordersbystatus")] //Получение заказов по статусу -Ok
        public async Task<IActionResult> GetOrdersByStatus([FromBody] GetOrderByStatus request)
        {

            if (ModelState.IsValid)
            {
                List<Order> filteredorders = await unitOfWork.Orders.GetAsync(request.orderStatus);
                if (filteredorders.Count>0)
                {
                    return Json(filteredorders);
                }
                else 
                {
                    return NotFound("Order is not found");                      
                }
            }
            else
            {
                return BadRequest("Incorrect input");
            }
        }

        [HttpGet("getorder")]
        public async Task<IActionResult> GetOrder(Guid orderId)
        {
            var order = await unitOfWork.Orders.GetAsync(orderId);
            if (order==null)
            {
                return NotFound("Not found the order");
            }
            else
            {
                return Json(order); 
            }
        }

        [HttpDelete("deleteorder")] //Удалить заказ - Ok
        public async Task<IActionResult> DeleteOrder(Guid orderId)
        {
            var order = await unitOfWork.Orders.GetAsync(orderId);
            if (order != null)
            {

                if (order.OrderStatus == OrderStatus.New)
                {
                    await unitOfWork.Orders.DeleteAsync(orderId);
                    return Ok();
                }
                else
                {
                    return BadRequest("Your order is processing or finished");
                }
            }
            else
            {
                return NotFound("Not found the order");
            }
        }

        [HttpGet("shopcart")]
        public async Task<IActionResult> GetShopCart()
        {
            bool iskey = HttpContext.Request.Cookies.ContainsKey("OrderId");
            if (iskey == false)
            {
                return BadRequest("You don't have any Items in your shopcart");
            }
            else
            {
                var OrderId = Guid.Parse(HttpContext.Request.Cookies["OrderId"]);
                var order = await unitOfWork.Orders.GetAsync(OrderId);
                return Json(order.OrderElements);
            }
        }

        [HttpPost("addtocart")] //Добавить товары в корзину  - Ok            
        public async Task<IActionResult> AddItemToShopCart([FromBody] AddItemsToShopCart requestmodel)
        {
            if (ModelState.IsValid)
            {     
                var item = await unitOfWork.Items.GetAsync(requestmodel.ItemId);             
                bool iskey = HttpContext.Request.Cookies.ContainsKey("OrderId");

                if (iskey == false) //ключ в куки не существует 
                {
                    Guid OrderId = Guid.NewGuid();
        
                    Order neworder = new Order()
                    {
                        CustomerId = (await unitOfWork.Customers.GetCurrentCustomer(HttpContext)).Id,
                        OrderId = OrderId,
                        OrderElements = new List<OrderElement>{new OrderElement(requestmodel.ItemId, requestmodel.ItemCount)
                        {
                            ItemPrice = item.Price * requestmodel.ItemCount,
                            OrderId = OrderId                         
                        }    
                      }
                    };
                    await unitOfWork.Orders.CreateAsync(neworder); 

                    HttpContext.Response.Cookies.Append("OrderId", Convert.ToString(neworder.OrderId));
                    return Ok("Вы создали корзину и положили туда "+ requestmodel.ItemCount+" товара");
                }
                else //ключ в куки существует
                {
                    Guid OrderId = Guid.Parse(HttpContext.Request.Cookies["OrderId"]);
                    var order = await unitOfWork.Orders.GetAsync(OrderId);

                    OrderElement orderElement = new OrderElement(requestmodel.ItemId, requestmodel.ItemCount)
                    {
                        ItemPrice = item.Price * requestmodel.ItemCount,
                        OrderId = OrderId,
                        Order = order,
                        OrderElementId = Guid.NewGuid()
                    };
                                                          
                    await unitOfWork.OrderElements.CreateAsync(orderElement);
                    order.OrderElements.Add(await unitOfWork.OrderElements.GetAsync(orderElement.OrderElementId)); 

                    return Ok("Вы добавили " + requestmodel.ItemCount + " товара в корзину");//a + "\nOrderId: " + OrderId);
                }
            }
            else
            {
                return BadRequest("Incorrect input");
            }
        }

        [HttpPost("makeorder")] // Сделать заказ
        public async Task<IActionResult> MakeOrder()
        {
            bool iskey = HttpContext.Request.Cookies.ContainsKey("OrderId");
            var customer = await unitOfWork.Customers.GetCurrentCustomer(HttpContext);
            if ( iskey == false)
            {
                return BadRequest("You don't have any Items in your shopcart");
            }
            else
            {
                Guid OrderId = Guid.Parse(HttpContext.Request.Cookies["OrderId"]);
                var order = await unitOfWork.Orders.GetAsync(OrderId);

                order.OrderNumber = new Generator().GetOrderNumber();                   
                order.OrderStatus = OrderStatus.New;
                order.OrderDate = DateTime.Now;
                await unitOfWork.SaveAsync();

                Response.Cookies.Delete("OrderId");
                return Ok("You've made order: №"+ order.OrderNumber); 
            }
        }
        
    }
}
