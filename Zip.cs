using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

public class Zip
{
    public void CompressDirectory(string directoryPath, string zipFilePath, RichTextBox CacheInfoBox)
    {
        try
        {
            if (Directory.Exists(directoryPath))
            {
                // Delete the temporary zip file if it already exists to avoid overwriting issues
                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);
                }

                // Create a zip file containing the directory contents
                ZipFile.CreateFromDirectory(directoryPath, zipFilePath);

                // Provide feedback to the user (e.g., in a RichTextBox)
                CacheInfoBox.AppendText($"Temporary zip file created at: {zipFilePath}\n");
            }
            else
            {
                CacheInfoBox.AppendText($"Directory does not exist: {directoryPath}\n");
            }
        }
        catch (IOException ex)
        {
            CacheInfoBox.AppendText($"Error while compressing directory: {ex.Message}\n");
        }
    }
}
