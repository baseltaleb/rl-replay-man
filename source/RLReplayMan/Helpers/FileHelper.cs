using System;
using System.IO;
using System.Windows;

namespace RLReplayMan
{
    static class FileHelper
    {
        public static bool DeleteFile(string filePath)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show(
                    "Are you sure you want to delete the selected file? ",
                    "Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    File.Delete(filePath);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                MessageBoxResult result = MessageBox.Show(
                    "File deletion faild: " + e.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
        }

        public static bool CopyFile(string fileName, string originalPath, string destinationPath)
        {
            try
            {
                var dest = Path.Combine(destinationPath, fileName);
                File.Copy(originalPath, dest);
                return true;
            }
            catch (Exception e)
            {
                MessageBoxResult result = MessageBox.Show(
                    "File copy faild: " + e.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }
        }
    }
}
