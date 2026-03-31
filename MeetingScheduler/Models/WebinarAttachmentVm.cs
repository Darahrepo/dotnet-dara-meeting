using AutoMapper;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Infrastructure.Common.Mappings;

namespace MeetingScheduler.UI.Models
{
    public class WebinarAttachmentVm : IMapFrom<WebinarAttachment>
    {
        public int Id { get; set; }
        public int WebinarId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        
        
        
        //Not Mapped
        public string Path {get;set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<WebinarAttachmentVm, WebinarAttachment>().ReverseMap();
            profile.CreateMap<WebinarAttachment, WebinarAttachmentVm>().ForMember(x => x.Path, opt => opt.Ignore());
            profile.CreateMap<WebinarAttachment, WebinarAttachmentVm>().ForMember(x => x.FileType, opt => opt.Ignore());
            profile.CreateMap<WebinarAttachment, WebinarAttachmentVm>().ForMember(x => x.FileSize, opt => opt.Ignore());
        }
    }
}
