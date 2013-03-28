<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FileModel>" %>
<%
    bool upperEnabled = Model.FullPath != "/";
    List<string> list = ViewData["CheckedList"] as List<string>;
    string jList = "";
    if (list != null && list.Count > 0)
    {
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        jList = serializer.Serialize(list);
    }
 
    { %>

     <div style="width:400px;padding-left: 200px; padding-top:10px;">
     <%= Html.Telerik().ComboBox()
            .Name("location")
            .AutoFill(true)
            .HtmlAttributes(new { style = "width:100%" })
            .Items(item =>
            {
                item.Add().Text(Model.FullPath == "/" ? "Computer" : Model.FullName).Value(Model.FullPath).Selected(true);
            })
            .DataBinding(binding => binding.Ajax().Select("LocationLoading", "Explorer", new {filePath = Model.FullPath }))
            .ClientEvents(events => events.OnChange("onLocationChange"))
            .HighlightFirstMatch(false)
    %>
    </div>
    <div style="position:absolute; top:0; padding-top:12px; padding-left:10px;">
     <a id="top" <% if (upperEnabled)
            { %> href='javascript:goTop();' <% 
        } %> style="padding-left: .4em;"><span>Top</span></a>
    |
    <a id="upper" <% if (upperEnabled)
            { %> href='javascript:goUpper("<%= Model.FullPath %>");' <% 
        } %> style="padding-left: .4em;"><span>Upper</span></a>
    |
    <a id="download" <% if (jList != "")
            { %> href='javascript:download();' <% 
        } %> style="padding-left: .4em;"><span>Download</span></a>
</div>


<%} %>
<script type="text/javascript">
    function goUpper(path) {
        $.ajax({
            type: "POST",
            url: getApplicationPath() + "Explorer/GoUpper",
            data: { path: path },
            cache: false,
            dataType: "json",
            success: function (data) {
                if (data != "") {
                    selectFolder(data);
                }
            }
        })
    }

    function goTop() {
        selectFolder("");
    }
    function download() {
        var list = '<%= jList %>';
        var path = getApplicationPath() + "Explorer/DownloadFiles/?jlist=" + list;
        window.location.href = path;
    }
    function onLocationChange() {
        var combobox = $(this).data('tComboBox');
        var inputfile = combobox.value();
        var curfile = '<%= Model.FullPath %>';
        if (inputfile.toLowerCase() != curfile.toLowerCase())
            selectFolder(inputfile);
    }
</script>
