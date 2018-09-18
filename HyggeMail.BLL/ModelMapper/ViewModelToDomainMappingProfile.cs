using AutoMapper;
using HyggeMail.BLL.Models;
using HyggeMail.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyggeMail.BLL.Infrastructure
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ViewModelToDomainMappings"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<UserModel, User>()
                .ForMember(x => x.UserID, map => map.Ignore())
                            .ForMember(x => x.ActivatedOn, map => map.Ignore())
        .ForMember(x => x.IsActivated, map => map.Ignore())
            .ForMember(x => x.ActivatedOn, map => map.Ignore());
            Mapper.CreateMap<AddUpdateAdminImageCategoryModel, ImageCategory>()
            .ForMember(x => x.ID, map => map.Ignore())
            .ForMember(x => x.IsActive, map => map.Ignore())
            .ForMember(x => x.IsDeleted, map => map.Ignore())
            .ForMember(x => x.AddedOn, map => map.Ignore())
            .ForMember(x => x.DeletedOn, map => map.Ignore());
            Mapper.CreateMap<AddUpdateAdminImageModel, AdminImage>()
                .ForMember(x => x.ID, map => map.Ignore())
                        .ForMember(x => x.IsActive, map => map.Ignore())
            .ForMember(x => x.IsDeleted, map => map.Ignore())
            .ForMember(x => x.AddedOn, map => map.Ignore())
            .ForMember(x => x.DeletedOn, map => map.Ignore());


            Mapper.CreateMap<AddUpdateRecipientModel, UserAddressBook>()
                .ForMember(x => x.UserIDFK, map => map.MapFrom(c => c.UserID))
    .ForMember(x => x.ID, map => map.Ignore())
            .ForMember(x => x.IsActive, map => map.Ignore())
.ForMember(x => x.IsDeleted, map => map.Ignore())
.ForMember(x => x.AddedOn, map => map.Ignore())
.ForMember(x => x.DeletedOn, map => map.Ignore());

            Mapper.CreateMap<ContactUsModel, ContactU>()
    .ForMember(x => x.ID, map => map.Ignore())
.ForMember(x => x.IsDeleted, map => map.Ignore())
.ForMember(x => x.IsResolved, map => map.Ignore())
.ForMember(x => x.ResolvedOn, map => map.Ignore())
.ForMember(x => x.AddedOn, map => map.Ignore())
.ForMember(x => x.DeletedOn, map => map.Ignore());


            Mapper.CreateMap<AddUpdateImageEditorModel, UserPostCard>()
 .ForMember(x => x.ID, map => map.Ignore())
.ForMember(x => x.IsDeleted, map => map.Ignore())
.ForMember(x => x.IsApproved, map => map.Ignore())
.ForMember(x => x.ApprovedOn, map => map.Ignore())
.ForMember(x => x.IsRejected, map => map.Ignore())
.ForMember(x => x.RejectedOn, map => map.Ignore())
.ForMember(x => x.ShipmentDate, map => map.Ignore())
.ForMember(x => x.ShippedOn, map => map.Ignore())
.ForMember(x => x.DeletedOn, map => map.Ignore());
            Mapper.CreateMap<UserRecipientModel, UserPostCardRecipient>()
            .ForMember(x => x.ID, map => map.Ignore())
            .ForMember(x => x.IsApproved, map => map.Ignore())
            .ForMember(x => x.IsCompleted, map => map.Ignore());
            Mapper.CreateMap<WebContactUsModel, ContactU>()
                .ForMember(x => x.ID, map => map.Ignore())
                .ForMember(x => x.IsDeleted, map => map.Ignore())
.ForMember(x => x.IsResolved, map => map.Ignore())
.ForMember(x => x.ResolvedOn, map => map.Ignore())
.ForMember(x => x.IsDeleted, map => map.Ignore())
.ForMember(x => x.DeletedOn, map => map.Ignore());
            Mapper.CreateMap<PostCardSelectedImages, UserPostCardImage>()
            .ForMember(x => x.ID, map => map.Ignore());
            Mapper.CreateMap<SubscriberModel, Subscriber>()
            .ForMember(x => x.ID, map => map.Ignore());
            Mapper.CreateMap<UserHistoryModel, UserHistory>()
                .ForMember(x => x.ID, map => map.Ignore());
        }
    }
}
