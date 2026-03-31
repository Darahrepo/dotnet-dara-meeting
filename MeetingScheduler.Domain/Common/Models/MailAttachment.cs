using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Common.Models
{
    public class MailAttachment
    {
        public Stream Stream { get; set; }
        public string FileName { get; set; }

        public MailAttachment(Stream stream, string fileName) => (Stream, FileName) = (stream, fileName);
    }
}

