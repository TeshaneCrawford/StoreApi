﻿namespace StoreApi.DTO
{
    public class UserDTO
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public CartDTO Cart { get; set; }
    }
}
