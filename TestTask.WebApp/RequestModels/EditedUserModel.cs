using System;

namespace TestTask.Requests
{
    public class EditedUserModel
    {
        public Guid EditedUserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public float Discount { get; set; }

    }
}
