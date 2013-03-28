<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div id="searchbox">
    <label for="search-label">
        Search</label>
    <%= Html.TextBox("searchinput", "", new {onchange="search()" })%>
    <a id="searchlink" href="javascript:void(0);">
        <img height="14" width="14" runat="server" alt="search icon" style="border: none"
            src="~/Content/Images/Search.png" /></a> <a id="clearlink" href="javascript:clear();">
                <img height="14" width="14" runat="server" alt="search icon" style="border: none"
                    src="~/Content/Images/Clear.png" /></a>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        var searchinput = $('#searchinput');
        var searchText = '<%=Model %>';
        if (searchText != "") {
            if (searchText != searchinput.val())
                searchinput.val(searchText);
            $('#grid-search label').hide();
            $('#searchlink').hide();
        } else {
            $('#clearlink').hide();
        }
    });

    function search() {
        var text = $('#searchinput').val();
        var combobox = $("#location").data('tComboBox');
        var path = combobox.value();

        if (text == "")
            showListView(path);
        else
            showSearchView(path, text);
    }

    function showSearchView(path, searchtext) {
        $.ajax({
            type: "POST",
            url: getApplicationPath() + "Search/SearchView",
            data: { folderPath: path, searchString: searchtext },
            cache: false,
            dataType: "html",
            success: function (data) {
                $('#searchlink').hide();
                $('#clearlink').show();

                $('#listpanel').html(data);

                $('#searchinput').blur();
                $('#searchinput').focus();
            },
            error: function (req, status, error) {
                alert("switch to search view failed!");
            }
        });
    }


    function showListView(path) {
         $.ajax({
            type: "POST",
            url: getApplicationPath() + "Explorer/FileList",
            data: { folderPath: path },
            cache: false,
            dataType: "html",
            success: function (data) {
                $('#searchlink').show();
                $('#clearlink').hide();

                $('#listpanel').html(data);

                $('#searchinput').blur();
                $('#searchinput').focus();
            },
            error: function (req, status, error) {
                alert("switch to normal view failed!");
            }
        });
    }

    function clear() {
        $("#searchinput").val("");
        search();
    }

    $('#searchbox input')
               .keydown(function (e) {
                   if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
                       $('#searchinput').blur();
                       $('#searchinput').focus();
                       return false;
                   }
                   else
                       if ((e.which && e.which == 27) || (e.keyCode && e.keyCode == 27)) {
                           clear();
                       }
                   return true;
               });

</script>
