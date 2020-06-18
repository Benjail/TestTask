using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestTask.Models;
using System;
using System.Threading.Tasks;
using TestTask.Requests;
using TestTask.Services;
using TestTask.WebApp.Repositories.Interfaces;

namespace TestTask.Controllers
{
    [Authorize(AuthenticationSchemes = "Cookies", Roles = "Admin")]
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<Customer> userManager;       

       public AdminController(UserManager<Customer> userManager,  IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("item")] //вывести все товары - Ok
        public async Task<IActionResult> GetItem(Guid guid)
        {
            if (ModelState.IsValid)
            {
                if ((await unitOfWork.Items.GetAsync(guid) != null))
                {
                    return Json(await unitOfWork.Items.GetAsync(guid));
                }
                else
                {
                    return NotFound("Item is not found");
                }
            }
            else
            {
                return BadRequest("Request model is invalid");
            }

        }

        [HttpGet("allitems")] //вывести все товары - Ok
        public async Task<IActionResult> GetAllItems()
        {
            return Json(await unitOfWork.Items.GetAllAsync());       
        }

        [HttpDelete("itemdelete")] //удаление товара по ID - Ok
        public async Task<IActionResult> ItemDelete(Guid guid)
        {
            if (ModelState.IsValid)
            {
                if ((await unitOfWork.Items.GetAsync(guid) != null))
                {
                    await unitOfWork.Items.DeleteAsync(guid);
                    return Ok();
                }
                else
                {
                    return NotFound("Item is not found");        
                }
            }
            else
            {
                return BadRequest("Request model is invalid");
            }

        } 

        [HttpPost("additem")] //добавление товара - Ok 
        public async Task<IActionResult> AddItem([FromBody] AddItemModel request)
        {
            if (ModelState.IsValid)
            {
                var item = new Item()
                {
                    Name = request.Name,
                    Price = request.Price,
                    Category = request.Category,
                    Code = request.Code
                };
                await unitOfWork.Items.CreateAsync((item));                         
                return Ok();
            }
            else
            {
                return BadRequest("Request model is invalid");
            }
        }
       
        [HttpPost("updateitem")] //обновление товара -Ok
        public async Task<IActionResult> UpdateItem([FromBody]EditedItemModel request)
        {
            if (ModelState.IsValid)
            {
                if ((await unitOfWork.Items.GetAsync(request.EditedItemId) != null))
                {
                    var item = new Item()
                    {
                        ItemId = request.EditedItemId,
                        Name = request.Name,
                        Price = request.Price,
                        Category = request.Category,
                        Code = request.Code
                    };
                    await unitOfWork.Items.Update(item);
                    return Ok();
                }
                else
                {
                    return NotFound("Item is not found");
                }
            }
            else
            {
                return BadRequest("Request model is invalid");
            }
        }

        [HttpGet("getallcustomers")] // Получить всех пользователей- Ok 
        public async Task<IActionResult> GetAllCustomers() 
        {
            return Json(await unitOfWork.Customers.GetAllAsync());      
        }

        [HttpGet("customer")]
        public async Task<IActionResult> GetCustomer(Guid guid) //получить пользователя по Id- Ok
        {
            if (ModelState.IsValid)
            {
                if (await unitOfWork.Customers.GetAsync(guid) != null)
                {
                    return Json(await unitOfWork.Customers.GetAsync(guid));
                }
                else
                {
                    return NotFound("Customer not found");                      
                }
            }
            else
            {
                return BadRequest("Request model is invalid");
            }
        }
        [HttpDelete("customerdelete")]
         public async Task<IActionResult> DeleteCustomer(Guid guid) //удалить пользователя по Id- Ok
         {
            if (ModelState.IsValid)
            {
                if (await unitOfWork.Customers.GetAsync(guid) != null)
                {
                    var customer = await unitOfWork.Customers.GetAsync(guid);
                    await unitOfWork.Customers.DeleteAsync(customer);
                    return Ok();
                }
                else
                {
                    return NotFound("Customer not found");
                }
            }
            else
            {
                return BadRequest("Request model is invalid");
            }
         }

        [HttpPost("customeradd")] //добавление пользователя - Ok
        public async Task<IActionResult> AddCustomer([FromBody] AddUserModel request)
        {
            if (ModelState.IsValid)
            {
                if (await unitOfWork.Customers.GetAsync(request.Email) == null)
                {
                    var customer = new Customer()
                    {
                        Name = request.Name,
                        UserName = request.Email,
                        Email = request.Email,
                        Address = request.Address,
                        Code = request.Code,
                        Discount = request.Discount
                    };
                    string pass = new Generator().GetPass();
                    await unitOfWork.Customers.CreateAsync(customer,pass );
                    return Ok(pass); 
                }
                else
                {
                    return BadRequest("This user already exists");
                }
            }
            else
            {
                return BadRequest("Request model is invalid");
            }
        }
        
        [HttpPost("updatecustomer")] //обновление пользователя - Ok
        public async Task<IActionResult> UpdateCustomer([FromBody] EditedUserModel request)
        {
            if (ModelState.IsValid)
            {
                if (await unitOfWork.Customers.GetAsync(request.EditedUserId) != null)
                {
                        var customer = await unitOfWork.Customers.GetAsync(request.EditedUserId);
                        {
                            customer.Address = request.Address;
                            customer.Email = request.Email;
                            customer.UserName = request.Email; 
                            customer.Code = request.Code;
                            customer.Name = request.Name;
                            customer.Discount = request.Discount;
                        }
                        await unitOfWork.Customers.Update(customer);
                        return Ok();
                }
                else
                {
                    return NotFound("Customer not found");
                }
            }
            else 
            {
                return BadRequest("The request model is invalid");
            }
           
        }
        
        [HttpPost("confirmorder")] //подтверждение заказа
        public async Task<IActionResult> ConfirmOrder([FromBody] ConfirmOderModel request)
        {
            if (ModelState.IsValid)
            {
                if (await unitOfWork.Orders.GetAsync(request.OrderId) != null)
                {
                    (await unitOfWork.Orders.GetAsync(request.OrderId)).ShipmentDate = DateTime.Now;
                    (await unitOfWork.Orders.GetAsync(request.OrderId)).OrderStatus = OrderStatus.InProceed;
                    await unitOfWork.SaveAsync();
                    return Ok();
                }
                else
                {
                    return NotFound("Order not found");
                }
            }
            return BadRequest("The request model is invalid");                  
        }
         
        //[HttpPost("closeorder")]
        [HttpPost("closeorder")]//закрытие заказа
        public async Task<IActionResult> CloseOrder(Guid orderId)
        {
            if (ModelState.IsValid)
            {
                if (await unitOfWork.Orders.GetAsync(orderId) != null)
                {
                    (await unitOfWork.Orders.GetAsync(orderId)).OrderStatus = OrderStatus.Finished;
                    await unitOfWork.SaveAsync();
                    return Ok();
                }
                else
                {
                    return NotFound("Order not found");
                }
            }
            else
            {
                return BadRequest("Request model is invalid");
            }
        }
    }
}