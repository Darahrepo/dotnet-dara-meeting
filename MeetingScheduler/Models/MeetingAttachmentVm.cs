using AutoMapper;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Infrastructure.Common.Mappings;

namespace MeetingScheduler.UI.Models
{
    public class MeetingAttachmentVm : IMapFrom<MeetingAttachment>
    {
        public int Id { get; set; }
        public int MeetingId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        
        
        
        //Not Mapped
        public string Path {get;set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<MeetingAttachmentVm, MeetingAttachment>().ReverseMap();
            profile.CreateMap<MeetingAttachment, MeetingAttachmentVm>().ForMember(x => x.Path, opt => opt.Ignore());
            profile.CreateMap<MeetingAttachment, MeetingAttachmentVm>().ForMember(x => x.FileType, opt => opt.Ignore());
            profile.CreateMap<MeetingAttachment, MeetingAttachmentVm>().ForMember(x => x.FileSize, opt => opt.Ignore());
        }
    }
}
