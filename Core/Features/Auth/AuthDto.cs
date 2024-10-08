﻿using System.Text.Json.Serialization;

namespace Core.Features.Auth
{
    public class AuthDto
    {
        public string? Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public List<string>? Roles { get; set; }
    }
}
