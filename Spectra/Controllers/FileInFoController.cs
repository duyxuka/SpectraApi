using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Spectra.Models;
using static System.Net.Mime.MediaTypeNames;
using Spectra.Services;
using System.Threading;
using Microsoft.AspNetCore.Authorization;

namespace Spectra.Controllers
{
    [EnableCors("AddCors")]
    [Route("api/[controller]")]
    [ApiController]

    public class FileInFoController : ControllerBase
    {
        private readonly FileServices _fileService;

        public FileInFoController(FileServices fileService)
        {
            _fileService = fileService;
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Ok ");
        }



        // download file(s) to client according path: rootDirectory/subDirectory with single zip file
        [HttpGet("Download/{subDirectory}")]
        public IActionResult DownloadFiles(string subDirectory)
        {
            try
            {
                var (fileType, archiveData, archiveName) = _fileService.FetechFiles(subDirectory);

                return File(archiveData, fileType, archiveName);
            }
            catch (Exception exception)
            {
                return BadRequest($"Error: {exception.Message}");
            }
        }

        // upload file(s) to server that palce under path: rootDirectory/subDirectory
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm(Name = "file")] List<IFormFile> files, string subDirectory)
        {
            try
            {
                var savedFileNames = await _fileService.SaveFileAsync(files, subDirectory);

                return Ok(new
                {
                    files.Count,
                    Size = FileServices.SizeConverter(files.Sum(f => f.Length)),
                    SavedFileNames = savedFileNames // Trả về danh sách tên file đã lưu
                });
            }
            catch (Exception exception)
            {
                return BadRequest($"Error: {exception.Message}");
            }
        }

        [HttpPost("uploadUser")]
        public async Task<IActionResult> UploadFileUser([FromForm(Name = "file")] List<IFormFile> files, string subDirectory)
        {

            try
            {
                var savedFileNames = await _fileService.SaveFileUserAsync(files, subDirectory);

                return Ok(new
                {
                    files.Count,
                    Size = FileServices.SizeConverter(files.Sum(f => f.Length)),
                    SavedFileNames = savedFileNames // Trả về danh sách tên file đã lưu
                });
            }
            catch (Exception exception)
            {
                return BadRequest($"Error: {exception.Message}");
            }
        }

        [HttpDelete("delete-file")]
        public async Task<IActionResult> DeleteFile([FromQuery] string fileName, [FromQuery] string subDirectory)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(subDirectory))
            {
                return BadRequest("File name and sub-directory must be provided.");
            }

            try
            {
                var result = await _fileService.DeleteFileAsync(fileName, subDirectory);

                if (result)
                {
                    return Ok(new { message = "File deleted successfully." });
                }
                else
                {
                    return NotFound(new { message = "File not found." });
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi (nếu cần) và trả về thông báo lỗi
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        [HttpGet("check-file-exists")]
        public IActionResult CheckFileExists([FromQuery] string fileName, [FromQuery] string subDirectory)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(subDirectory))
            {
                return BadRequest("File name and sub-directory must be provided.");
            }

            try
            {
                var filePath = Path.Combine("wwwroot", subDirectory, fileName);
                var exists = System.IO.File.Exists(filePath);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }        
    }
}
