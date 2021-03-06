﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsSearchController
    {
        [HttpPost, Route(RouteTree)]
        public async Task<ActionResult<TreeResult>> Tree([FromBody]TreeRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ContentsSearch))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var root = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                return new
                {
                    Count = count
                };
            });

            if (!request.Reload)
            {
                var siteUrl = await _pathManager.GetSiteUrlAsync(site, true);
                var groupNames = await _contentGroupRepository.GetGroupNamesAsync(request.SiteId);
                var tagNames = await _contentTagRepository.GetTagNamesAsync(request.SiteId);
                var checkedLevels = ElementUtils.GetCheckBoxes(CheckManager.GetCheckedLevels(site, true, site.CheckContentLevel, true));

                var columnsManager = new ColumnsManager(_databaseManager, _pluginManager);
                var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.SearchContents);
                var permissions = new Permissions
                {
                    IsAdd = await auth.AdminPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentAdd),
                    IsDelete = await auth.AdminPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentDelete),
                    IsEdit = await auth.AdminPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentEdit),
                    IsArrange = await auth.AdminPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentArrange),
                    IsTranslate = await auth.AdminPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentTranslate),
                    IsCheck = await auth.AdminPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ContentCheckLevel1),
                    IsCreate = await auth.AdminPermissions.HasSitePermissionsAsync(site.Id, Constants.SitePermissions.CreateContents) || await auth.AdminPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.CreatePage),
                    IsChannelEdit = await auth.AdminPermissions.HasChannelPermissionsAsync(site.Id, channel.Id, Constants.ChannelPermissions.ChannelEdit)
                };

                return new TreeResult
                {
                    Root = root,
                    SiteUrl = siteUrl,
                    GroupNames = groupNames,
                    TagNames = tagNames,
                    CheckedLevels = checkedLevels,
                    Columns = columns,
                    Permissions = permissions
                };
            }

            return new TreeResult
            {
                Root = root
            };
        }

        public class TreeRequest : SiteRequest
        {
            public bool Reload { get; set; }
        }

        public class Permissions
        {
            public bool IsAdd { get; set; }
            public bool IsDelete { get; set; }
            public bool IsEdit { get; set; }
            public bool IsArrange { get; set; }
            public bool IsTranslate { get; set; }
            public bool IsCheck { get; set; }
            public bool IsCreate { get; set; }
            public bool IsChannelEdit { get; set; }
        }

        public class TreeResult
        {
            public Cascade<int> Root { get; set; }
            public string SiteUrl { get; set; }
            public IEnumerable<string> GroupNames { get; set; }
            public IEnumerable<string> TagNames { get; set; }
            public IEnumerable<CheckBox<int>> CheckedLevels { get; set; }
            public List<ContentColumn> Columns { get; set; }
            public Permissions Permissions { get; set; }
        }
    }
}
