using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Common.Services
{
    public class FileServices : IFileServices
    {
        private readonly string _fileRoot;
        private readonly string _resourcesPath;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileServices(string fileRoot,  IHttpContextAccessor httpContextAccessor)
        {
            //_resources = "Resources";
            _fileRoot = Path.Combine(fileRoot);

            _httpContextAccessor = httpContextAccessor;
        }
        public async Task DeleteFile(string filePath)
        {
            var path = Path.Combine(_fileRoot, FilesConstants.WebinarFilesLocation, filePath);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        public async Task<bool> FileExists(string filePath)
        {
            var path = Path.Combine(_fileRoot, FilesConstants.WebinarFilesLocation, filePath).Replace('\\', '/');

            return await Task.FromResult(File.Exists(path));
        }
        public async Task<Stream> GetFile( string filePath)
        {
            var path = Path.Combine(_fileRoot,filePath);
            Stream stream = null;

            if (File.Exists(path))
            {
                stream = File.OpenRead(path);
            }

            return await Task.FromResult(stream);
        }
        public async Task<string> GetFileUrl(string filePath)
        {
            var httpRequest = _httpContextAccessor.HttpContext.Request;
            var url = ($"{_fileRoot}\\{filePath.Replace('/', '\\')}");
            //var url = Path.Combine(_fileRoot, filePath);
            return await Task.FromResult(url);
        }

        public async Task<string> SaveFile( string filePath, Stream fileStream)
        {
            string dynamicFileName = GetDynamicFileName(filePath);
            var path = Path.Combine(_fileRoot, FilesConstants.WebinarFilesLocation, dynamicFileName);

            var directoryPath = Path.GetDirectoryName(path);

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using var file = new FileStream(path, FileMode.CreateNew);
            await fileStream.CopyToAsync(file);

            return Path.Combine(FilesConstants.WebinarFilesLocation, dynamicFileName).Replace('\\', '/');
        }

        public async Task EnsurePathExists( string filePath)
        {
            var path = Path.Combine(_fileRoot, filePath);
            var directoryPath = Path.GetDirectoryName(path);

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }

        public async Task GetFileMimeType(string filePath)
        {
            var path = Path.Combine(_fileRoot, filePath);
            var directoryPath = Path.GetDirectoryName(path);

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }

        private string GetDynamicFileName(string filePath)
        {
            var fileExtension = Path.GetExtension(filePath);
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var dynamicFileName = filePath.Replace(fileName + fileExtension, Guid.NewGuid() + fileExtension);
            return dynamicFileName;
        }

    }
}
