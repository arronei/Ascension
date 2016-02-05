<%@ Page Language="C#" Debug="true" %>
<%@ Import Namespace="System.IO" %>

<script language="c#" runat="server">

protected void Page_Load(object sender, System.EventArgs e)
{
    try {
        string filename = Request.Params["filename"];
        string localPath = Path.GetFileName(Request.Params["filename"]);
        if (string.IsNullOrEmpty(localPath)) {
            Response.Write("No filename provided.  Exiting.");
            return;
        }

        string targetPath = Path.Combine(
                Server.MapPath("./Data/"),
                localPath);

        int totalBytes = 0;
        using (Stream input = Request.InputStream)
        using (FileStream output = File.Create(targetPath)) {
            Byte[] buffer = new Byte[32 * 1024];
            while (true) {
                int bytesRead = input.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) {
                    break;
                }

                output.Write(buffer, 0, bytesRead);
                totalBytes += bytesRead;
            }
        }
      
        Response.Write(
            string.Format(
                @"Successfully wrote <a href=""./Data/{0}"">{0}</a>. ({1} bytes)",
                filename,
                totalBytes));
    }
    catch (Exception ex) {
        Response.Write("Server Error: " + ex.ToString());
    }
}

</script>