﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DasBlog.Web.Models;
using DasBlog.Web.Settings;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Core;
using DasBlog.Managers.Interfaces;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Web.Controllers
{
    public class HomeController : DasBlogBaseController
	{
        private readonly IBlogManager _blogManager;
        private readonly IDasBlogSettings _dasBlogSettings;
		private readonly IMapper _mapper;

		public HomeController(IBlogManager blogManager, IDasBlogSettings settings, IMapper mapper) : base(settings)
        {
            _blogManager = blogManager;
            _dasBlogSettings = settings;
			_mapper = mapper;
		}

		public IActionResult Index()
        {
			ListPostsViewModel lpvm = new ListPostsViewModel();
            lpvm.Posts = _blogManager.GetFrontPagePosts()
                            .Select(entry => _mapper.Map<PostViewModel>(entry)).ToList();
            DefaultPage();

            return ThemedView("Page", lpvm);
        }

		[HttpGet("page")]
        public IActionResult Page()
        {
            return Index();
        }

        [HttpGet("page/{index:int}")]
        public IActionResult Page(int index)
        {
            if (index == 0)
            {
                return Index();
            }

            ViewData["Message"] = string.Format("Page...{0}", index);

            ListPostsViewModel lpvm = new ListPostsViewModel();
            lpvm.Posts = _blogManager.GetEntriesForPage(index)
                                .Select(entry => _mapper.Map<PostViewModel>(entry)).ToList();

            DefaultPage();

			return ThemedView("Page", lpvm);
        }

        [HttpGet("blogger")]
        public ActionResult Blogger()
        {
            // https://www.poppastring.com/blog/blogger.aspx
            // Implementation of Blogger XML-RPC Api
            // blogger
            // metaWebLog
            // mt

            return NoContent();
        }

        [Produces("text/xml")]
		[HttpPost("blogger")]
		public IActionResult Blogger([FromBody] string xmlrpcpost)
        {
            return this.Content(_blogManager.XmlRpcInvoke(this.HttpContext.Request.Body));
        }

        public IActionResult About()
        {
            DefaultPage();

            ViewData["Message"] = "Your application description page.";

            return NoContent();
        }

        public IActionResult Contact()
        {
            DefaultPage();

            ViewData["Message"] = "Your contact page.";

            return NoContent();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

