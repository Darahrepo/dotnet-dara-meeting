using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Common.Interfaces
{
    public interface IFileServices
    {
        Task DeleteFile(string filePath);
        Task<bool> FileExists(string filePath);
        Task<Stream> GetFile(string filePath);
        Task<string> GetFileUrl(string filePath);
        Task<string> SaveFile(string filePath, Stream fileStream);
        Task EnsurePathExists(string filePath);
    }
}
