﻿using Microsoft.AspNetCore.Http;

namespace NET6.Entity.DTOs.Abouts
{
    public class AboutAddDto
    {
        public string Title { get; set; }
        public string Job { get; set; }
        public string Content { get; set; }
        public IFormFile Photo { get; set; }

    }
}
