<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FileModel>" %>
<%
    IList<FileModel> data = new List<FileModel>();
    data.Add(Model);
    
    Html.Telerik().TreeView()
         .Name("folderList")
          .BindTo(data, items =>
          {
              items.For<FileModel>(binding => binding
                      .ItemDataBound((item, file) =>
                      {
                          item.Text = file.Name;
                          item.Value = file.FullPath;
                          item.ImageUrl = Url.Content("~/Content/Images/" + file.Category.ToString() + ".png");
                          item.LoadOnDemand = true;
                       })

                      .Children(file => file.SubFolders))
                      ;

          })

          .DataBinding(dataBinding => dataBinding
                  .Ajax().Select("SubFoldersLoading", "Explorer")
          )
          .ClientEvents(events => events.OnSelect("onFolderSelect"))
          .ExpandAll(true)
          .Render();
%>

<script type="text/javascript">
function onFolderSelect(e) {
    var tv = $('#folderList').data('tTreeView');
    var file = tv.getItemValue(e.item);
    selectFolder(file);
}
</script>