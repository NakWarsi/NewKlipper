﻿
namespace Models.Core.Authentication
{
    public class User
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
    }
}