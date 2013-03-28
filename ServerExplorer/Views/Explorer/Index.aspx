<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FileModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    ServerExplorer
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="listpanel">
        <% Html.RenderPartial("FileList", Model.SubFolders.FirstOrDefault());%>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="NavContent" runat="server">
    <div id="header">
        <% Html.Telerik().PanelBar()
            .Name("titlebar")
            .Items(title =>
            {
                title.Add()
                    .Text("Server Explorer")
                    .Content(() =>
                    {%>
        <% Html.RenderPartial("~/Plugins/SearchBoxPlugin.dll/SearchBoxPlugin.Views.Search.SearchBox.ascx", "");%>
        <div id="commands">
            
            <% Html.RenderPartial("ActionLinks", Model.SubFolders.FirstOrDefault());%>
            
        </div>
        <%})
                  .Expanded(true);
            })
          .Render();
        %>
    </div>
    <% Html.RenderPartial("Navigation");%>
</asp:Content>
