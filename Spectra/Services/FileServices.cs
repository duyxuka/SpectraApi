using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
namespace Spectra.Services
{
    public class FileServices
    {
        //public void SaveFile(List<IFormFile> files, string subDirectory)
        //{
        //    subDirectory = subDirectory ?? string.Empty;
        //    var target = Path.Combine("wwwroot\\images", subDirectory);

        //    Directory.CreateDirectory(target);

        //    files.ForEach(async file =>
        //    {
        //        if (file.Length <= 0) return;
        //        var filePath = Path.Combine(target, file.FileName);
        //        try
        //        {
        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await file.CopyToAsync(stream);
        //            }

        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //        }
        //    });
        //}

        public async Task<List<string>> SaveFileAsync(List<IFormFile> files, string subDirectory)
        {
            subDirectory = subDirectory ?? string.Empty;
            var target = Path.Combine("wwwroot\\images", subDirectory);

            Directory.CreateDirectory(target);

            var savedFileNames = new List<string>();

            foreach (var file in files)
            {
                if (file.Length <= 0) continue;

                // Tạo tên file mới bằng cách thêm GUID trước tên file gốc
                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(target, uniqueFileName);

                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Thêm tên file đã lưu vào danh sách để lưu vào database
                    savedFileNames.Add(uniqueFileName);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return savedFileNames; // Trả về danh sách tên file đã lưu
        }

        public async Task<List<string>> SaveFileUserAsync(List<IFormFile> files, string subDirectory)
        {
            subDirectory = subDirectory ?? string.Empty;
            var target = Path.Combine("wwwroot\\images", subDirectory);

            Directory.CreateDirectory(target);

            var savedFileNames = new List<string>();

            foreach (var file in files)
            {
                if (file.Length <= 0) continue;

                // Tạo tên file mới bằng cách thêm GUID trước tên file gốc
                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(target, uniqueFileName);

                try
                {
                    using (var image = Image.Load(file.OpenReadStream()))
                    {
                        string newSize = ResizeImage(image, 700, 800);
                        string[] aSize = newSize.Split(',');
                        image.Mutate(h => h.Resize(Convert.ToInt32(aSize[1]), Convert.ToInt32(aSize[0])));
                        image.Save(filePath);
                    }

                    // Thêm tên file đã lưu vào danh sách để lưu vào database
                    savedFileNames.Add(uniqueFileName);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return savedFileNames; // Trả về danh sách tên file đã lưu
        }


        //public void SaveFileUser(List<IFormFile> files, string subDirectory)
        //{
        //    subDirectory = subDirectory ?? string.Empty;
        //    var target = Path.Combine("wwwroot\\images", subDirectory);

        //    Directory.CreateDirectory(target);
        //    files.ForEach(file =>
        //    {
        //        if (file.Length <= 0) return;
        //        var filePath = Path.Combine(target, file.FileName);
        //        try
        //        {
        //            using (var image = Image.Load(file.OpenReadStream()))
        //            {
        //                string newSize = ResizeImage(image, 700, 800);
        //                string[] aSize = newSize.Split(',');
        //                image.Mutate(h => h.Resize(Convert.ToInt32(aSize[1]), Convert.ToInt32(aSize[0])));
        //                image.Save(filePath);
        //            }

        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //        }
        //    });
        //}


        public async Task<bool> DeleteFileAsync(string fileName, string subDirectory)
        {
            try
            {
                // Tạo đường dẫn đầy đủ tới file
                var filePath = Path.Combine("wwwroot", subDirectory, fileName);

                if (File.Exists(filePath))
                {
                    // Xóa file
                    File.Delete(filePath);
                    return await Task.FromResult(true);
                }

                // File không tồn tại
                return await Task.FromResult(false);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi (nếu cần) và ném ngoại lệ để xử lý ở tầng trên
                Console.WriteLine($"Error deleting file: {ex.Message}");
                throw;
            }
        }
        public string ResizeImage(Image img, int maxWdith, int maxHeight)
        {
            if(img.Width > maxWdith || img.Height > maxHeight)
            {
                double widthRatio = (double)img.Width / (double)maxWdith;
                double heightRatio = (double)img.Height / (double)maxHeight;
                double ratio = Math.Max(widthRatio, heightRatio);
                int newWidth = (int)(img.Width / ratio);
                int newHeight = (int)(img.Height / ratio);
                return newHeight.ToString() + "," + newWidth.ToString();
            }
            else
            {
                return img.Height.ToString() + "," + img.Width.ToString();
            }
        }
        public (string fileType, byte[] archiveData, string archiveName) FetechFiles(string subDirectory)
        {
            var zipName = $"archive-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";

            var files = Directory.GetFiles(Path.Combine("wwwroot\\images", subDirectory)).ToList();

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    files.ForEach(file =>
                    {
                        var theFile = archive.CreateEntry(file);
                        using (var streamWriter = new StreamWriter(theFile.Open()))
                        {
                            streamWriter.Write(File.ReadAllText(file));
                        }

                    });
                }

                return ("application/zip", memoryStream.ToArray(), zipName);
            }


        }

        public static string SizeConverter(long bytes)
        {
            var fileSize = new decimal(bytes);
            var kilobyte = new decimal(1024);
            var megabyte = new decimal(1024 * 1024 * 1024);
            var gigabyte = new decimal(1024 * 1024 * 1024);

            switch (fileSize)
            {
                case var _ when fileSize < kilobyte:
                    return $"Less then 1KB";
                case var _ when fileSize < megabyte:
                    return $"{Math.Round(fileSize / kilobyte, 0, MidpointRounding.AwayFromZero):##,###.##}KB";
                case var _ when fileSize < gigabyte:
                    return $"{Math.Round(fileSize / megabyte, 2, MidpointRounding.AwayFromZero):##,###.##}MB";
                case var _ when fileSize >= gigabyte:
                    return $"{Math.Round(fileSize / gigabyte, 2, MidpointRounding.AwayFromZero):##,###.##}GB";
                default:
                    return "n/a";
            }
        }
    }
}
