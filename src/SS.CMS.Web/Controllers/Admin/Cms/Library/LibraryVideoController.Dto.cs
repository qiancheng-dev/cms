﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Cms.Library
{
    public partial class LibraryVideoController
    {
        public class QueryRequest
        {
            public int SiteId { get; set; }
            public string Keyword { get; set; }
            public int GroupId { get; set; }
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class QueryResult
        {
            public IEnumerable<LibraryGroup> Groups { get; set; }
            public int Count { get; set; }
            public IEnumerable<LibraryVideo> Items { get; set; }
        }

        public class CreateRequest : SiteRequest
        {
            public int GroupId { get; set; }
            public IFormFile File { get; set; }
        }

        public class DownloadRequest : SiteRequest
        {
            public int LibraryId { get; set; }
        }

        public class GroupRequest
        {
            public int SiteId { get; set; }
            public string Name { get; set; }
        }

        public class UpdateRequest : SiteRequest
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public int GroupId { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public int Id { get; set; }
        }
    }
}