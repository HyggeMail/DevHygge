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
    public class DomainToViewModelMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DomainToViewModelMappings"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<User, UserModel>();
            Mapper.CreateMap<EmailTemplate, EmailTemplateModel>();
            Mapper.CreateMap<ImageCategory, AddUpdateAdminImageCategoryModel>();
            Mapper.CreateMap<ImageCategory, AdminImageCategoryListingModel>();
            Mapper.CreateMap<AdminImage, AddUpdateAdminImageModel>();
            Mapper.CreateMap<AdminImage, AdminImageListingModel>()
                .ForMember(x => x.CategoryName, map => map.MapFrom(x => x.ImageCategory.CategoryName));
            Mapper.CreateMap<UserAddressBook, AddUpdateRecipientModel>()
                .ForMember(x => x.UserID, map => map.MapFrom(x => x.UserIDFK));
            Mapper.CreateMap<UserAddressBook, RecipientListingModel>()
                .ForMember(x => x.UserID, map => map.MapFrom(x => x.UserIDFK));
            Mapper.CreateMap<UserAddressBook, RecipientDetails>();
            Mapper.CreateMap<AdminImage, ImagesViewModel>()
                .ForMember(x => x.ImageID, map => map.MapFrom(x => x.ID))
                .ForMember(x => x.FileName, map => map.MapFrom(x => x.FileName))
                .ForMember(x => x.FilePath, map => map.MapFrom(x => x.FilePath));
            Mapper.CreateMap<UserPostCard, PostCardListingModel>()
    .ForMember(x => x.Username, map => map.MapFrom(x => string.Format("{0} {1}", x.User.FirstName, x.User.LastName)))
            .ForMember(x => x.orderStatus, map => map.MapFrom(x => (eOrderStatus)x.UserOrder.OrderStatus))
            .ForMember(x => x.OrderID, map => map.MapFrom(x => x.UserOrder.ID))
            .ForMember(x => x.CardBack, map => map.Ignore())
            .ForMember(x => x.CardFront, map => map.Ignore());

            Mapper.CreateMap<UserPostCardRecipient, PostCardListingModel>()
.ForMember(x => x.Username, map => map.MapFrom(x => string.Format("{0} {1}", x.UserPostCard.User.FirstName, x.UserPostCard.User.LastName)))
        .ForMember(x => x.OrderID, map => map.MapFrom(x => x.UserPostCard.UserOrder.ID))
        .ForMember(x => x.CardBack, map => map.Ignore())
        .ForMember(x => x.CardFront, map => map.Ignore());

            Mapper.CreateMap<UserPostCardRecipient, RecipientPostCardListingModel>()
                .ForMember(x => x.UserID, map => map.MapFrom(x => x.UserPostCard.UserID))
                .ForMember(x => x.ShipmentDate, map => map.MapFrom(x => x.UserPostCard.ShipmentDate))
                .ForMember(x => x.CardBackPath, map => map.MapFrom(x => x.UserPostCard.CardBackPath))
                .ForMember(x => x.IsCancel, map => map.MapFrom(x => x.UserPostCard.IsCancel))
                .ForMember(x => x.IsApproved, map => map.MapFrom(x => x.IsApproved ?? false))
                .ForMember(x => x.CardBackPath, map => map.MapFrom(x => x.UserPostCard.CardBackPath ?? ""))
                                .ForMember(x => x.CardFrontPath, map => map.MapFrom(x => x.UserPostCard.CardFrontPath ?? ""))
                                .ForMember(x => x.CardBackPathWithIFrame, map => map.MapFrom(x => x.UserPostCard.CardBackPathWithIFrame ?? null))
                .ForMember(x => x.IsCompleted, map => map.MapFrom(x => x.IsCompleted ?? false))
                .ForMember(x => x.CardFrontPath, map => map.MapFrom(x => x.UserPostCard.CardFrontPath))
            .ForMember(x => x.Username, map => map.MapFrom(x => string.Format("{0} {1}", x.UserPostCard.User.FirstName, x.UserPostCard.User.LastName)))
            .ForMember(x => x.IncludeAddress, map => map.MapFrom(x => x.UserPostCard.IncludeAddress))
            .ForMember(x => x.UserAddress, map => map.MapFrom(x => string.Format("{0}, {1}", x.UserPostCard.User.UserDetail.Address, x.UserPostCard.User.UserDetail.City)))
            .ForMember(x => x.NewLineAdress, map => map.MapFrom(x => string.Format("{0}, {1}, {2}", x.UserPostCard.User.UserDetail.State, x.UserPostCard.User.UserDetail.Country, x.UserPostCard.User.UserDetail.Zip)));

            Mapper.CreateMap<ContactU, WebContactUsModel>();
            Mapper.CreateMap<UserPostCard, AddUpdateImageEditorModel>()
                .ForMember(x => x.UserName, map => map.MapFrom(x => x.User.FirstName))
                .ForMember(x => x.UserAddress, map => map.MapFrom(x => string.Format("{0}, {1}, {2}, {3},", x.User.UserDetail.Address, x.User.UserDetail.City, x.User.UserDetail.State, x.User.UserDetail.Country, x.User.UserDetail.Zip)));
            Mapper.CreateMap<UserPostCardRecipient, UserRecipientModel>();

            Mapper.CreateMap<Subscriber, SubscriberModel>();
            Mapper.CreateMap<UserHistory, UserHistoryModel>();
            Mapper.CreateMap<UserPostCard, UserPostCardViewModel>();

        }
    }
}
