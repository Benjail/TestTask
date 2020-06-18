using System;

namespace TestTask.Services
{
    public class Generator
    {
        public string GetPass()
        {
            int[] arr = new int[16]; // сделаем длину пароля в 16 символов
            Random rnd = new Random();
            string Password = "";

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = rnd.Next(33, 125);                                 
                Password += (char)arr[i];                               
            }
            return Password;                                            
        }
        public int GetOrderNumber()
        {
            Random rnd = new Random();
            int OrderNumber = Convert.ToInt32(rnd.Next(1000000, 9999999));                                          
            return OrderNumber;                                         
        }
    }
}
