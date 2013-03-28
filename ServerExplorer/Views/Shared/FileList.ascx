<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FileModel>" %>

<% Html.Telerik().Grid<FileModel>()
        .Name("filelist")
        .DataKeys(key => key.Add(x => x.FullPath))
        .Columns(columns =>
        {
            columns.Bound(x => x.FullPath).Format("<input type='checkbox'  value='{0}'>").Encoded(false).Width(22).Title("");
            columns.Bound(x => x.Name).ClientTemplate("<img width='16' height='16' alt='<#= CategoryText #>' src='"
                                                        + Url.Content("~/Content/Images/")
                                                        + "<#= CategoryText #>.png'  style= 'vertical-align:middle;'/>"
                                                        + "<span id='<#= FullPath #>_span' style='padding-left: 2px;'> <#= Name #></span>")
                                       .Title("Name");
            columns.Bound(x => x.Created).Format("{0:g}").Title("Date created").ReadOnly(true);
            columns.Bound(x => x.Accessed).Format("{0:g}").Title("Date modified").ReadOnly(true);
            columns.Bound(x => x.IsFolder).Hidden(true);
            columns.Bound(x => x.FullPath).Hidden(true);

        })
        .DataBinding(dataBinding => dataBinding.Ajax()
                    .Select("SelectFiles", "Explorer", new { filePath = Model.FullPath })
                    )
        .Pageable(pager => pager.PageSize(Int32.MaxValue).Style(GridPagerStyles.Status))
        .Sortable(sorting => sorting.OrderBy(sortorder => sortorder.Add(x => x.Accessed).Descending()))
        .Selectable()
        .ClientEvents(events => events.OnRowSelect("onRowSelected").OnDataBound("onFileListDataBound"))
        .HtmlAttributes(new { style = "text-align:left; border:none;" })
        .Render();
%>
<script type="text/javascript">
    $(function () {
        $.extend($.fn.disableTextSelect = function () {
            return this.each(function () {
                if ($.browser.mozilla) {//Firefox
                    $(this).css('MozUserSelect', 'none');
                } else if ($.browser.msie) {//IE
                    $(this).bind('selectstart', function () { return false; });
                } else {//Opera, etc.
                    $(this).mousedown(function () { return false; });
                }
            });
        });

    });

    function onFileListDataBound(e) {
        var filename = '<%= Model.FullPath %>';
        $(':checkbox').click(function () {
            var list = new Object();
            var i = 0;
            var path = getApplicationPath();
            $("input:checkbox:checked").each(function () {
                list[i++] = $(this).val();
            });

            loadActionLinks(filename, list);
        });
        
        $('.noselect').disableTextSelect(); //No text selection on elements with a class of 'noSelect'
    }


    function onRowSelected(e) {
        var grid = $(this).data('tGrid');
        var filePath = e.row.cells[grid.columns.length - 1].innerHTML;
        var isFolder = e.row.cells[grid.columns.length - 2].innerHTML == "true";
        if (isFolder) {
            grid.rebind({ filePath: filePath });
            loadActionLinks(filePath);
        } else {
            path = getApplicationPath() + "Explorer/DownloadFile/?file=" + filePath;
            window.location.href = path;
        }
    }

</script>
