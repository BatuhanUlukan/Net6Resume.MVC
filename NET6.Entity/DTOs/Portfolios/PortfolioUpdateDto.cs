﻿using Microsoft.AspNetCore.Http;
using NET6.Entity.DTOs.Categories;
using NET6.Entity.Entities;

namespace NET6.Entity.DTOs.Portfolios
{
    public class PortfolioUpdateDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid CategoryId { get; set; }

        public Image Image { get; set; }
        public IFormFile? Photo { get; set; }

        public IList<CategoryDto> Categories { get; set; }
    }
}