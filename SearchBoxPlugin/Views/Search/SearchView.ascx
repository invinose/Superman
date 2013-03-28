<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<% 
    Html.Telerik().Grid<FileModel>()
       .Name("searchResult")
       .DataKeys(key => key.Add(x => x.FullPath))
       .Columns(columns =>
       {
           columns.Bound(x => x.Name).Title("")
                                     .ClientTemplate("<table cellpadding='0' cellspacing='0' class='content'>"
                                                      + "<tr><td width='24' rowspan='2'><img width='24' height='24' alt='<#= CategoryText #>' src='"
                                                      + Url.Content("~/Content/Images/")
                                                      + "<#= CategoryText #>.png'  style= 'vertical-align:middle;'/></td>"
                                                      + "<td><#= Name #></td></tr><td><#= DirectoryName #></td><tr></tr></table>");
           columns.Bound(x => x.Size).Format("Size: {0}").Title("");
           columns.Bound(x => x.Accessed).Format("Date Modified: {0:g}").Title("");
           columns.Bound(x => x.IsFolder).Hidden(true);
           columns.Bound(x => x.FullPath).Hidden(true);

       })
        .DataBinding(dataBinding => dataBinding.Ajax()
                   .Select("SearchResult", "Search", new { searchFolder = ViewBag.SearchFolder, searchString = ViewBag.SearchString })
                   )
       .Pageable(pager => pager.PageSize(Int32.MaxValue).Style(GridPagerStyles.Status))
       .HtmlAttributes(new { style = "text-align:left; border:none;" })
       .Render();
%>
