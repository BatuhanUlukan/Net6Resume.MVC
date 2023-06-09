﻿using Microsoft.AspNetCore.Mvc;
using NET6.Data.UnitOfWorks;
using NET6.Entity.Entities;
using NET6.Service.Services.Abstractions;
using NET6.Web.Models;
using System.Diagnostics;

namespace NET6.Web.Controllers
{
    public class ArticleController : Controller
    {
        private readonly ILogger<ArticleController> _logger;
        private readonly IArticleService articleService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUnitOfWork unitOfWork;

        public ArticleController(ILogger<ArticleController> logger, IArticleService articleService, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            this.articleService = articleService;
            this.httpContextAccessor = httpContextAccessor;
            this.unitOfWork = unitOfWork;
        }
        [HttpGet]
        [Route("article")]
        public async Task<IActionResult> Index(Guid? categoryId, int currentPage = 1, int pageSize = 3, bool isAscending = false)
        {
            var articles = await articleService.GetAllByPagingAsync(categoryId, currentPage, pageSize, isAscending);
            return View(articles);
        }
        [HttpGet]
        public async Task<IActionResult> Search(string keyword, int currentPage = 1, int pageSize = 3, bool isAscending = false)
        {
            var articles = await articleService.SearchAsync(keyword, currentPage, pageSize, isAscending);
            return View(articles);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Route("article/{title}/{id}")]
        public async Task<IActionResult> Detail(Guid id)
        {
            var ipAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var articeVisitors = await unitOfWork.GetRepository<ArticleVisitor>().GetAllAsync(null, x => x.Visitor, y => y.Article);
            var article = await unitOfWork.GetRepository<Article>().GetAsync(x => x.Id == id);

            var result = await articleService.GetArticleWithCategoryNonDeletedAsync(id);

            var visitor = await unitOfWork.GetRepository<Visitor>().GetAsync(x => x.IpAddress == ipAddress);

            var addArticleVisitors = new ArticleVisitor(article.Id, visitor.Id);

            if (articeVisitors.Any(x => x.VisitorId == addArticleVisitors.VisitorId && x.ArticleId == addArticleVisitors.ArticleId))
                return View(result);
            else
            {
                await unitOfWork.GetRepository<ArticleVisitor>().AddAsync(addArticleVisitors);
                article.ViewCount += 1;
                await unitOfWork.GetRepository<Article>().UpdateAsync(article);
                await unitOfWork.SaveAsync();
            }

            return View(result);
        }
    }
}