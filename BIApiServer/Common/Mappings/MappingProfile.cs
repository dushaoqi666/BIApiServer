using AutoMapper;
using BIApiServer.Models;
using BIApiServer.Models.Dtos;

namespace BIApiServer.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // 示例映射配置
            CreateMap<FileInfos, FileInfoDto>()
                .ForMember(dest => dest.FileName, 
                    opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.FileSize, 
                    opt => opt.MapFrom(src => src.Size))
                .ReverseMap();

            // 可以添加更多映射配置
            // CreateMap<Source, Destination>();
        }
    }
} 