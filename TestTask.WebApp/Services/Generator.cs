using System;

namespace TestTask.Services
{
    public class Generator
    {
        float OrderNumber = 0;
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
        public float GetOrderNumber()
        {

            OrderNumber += 1;
            return OrderNumber;
            
        }
    }
}
