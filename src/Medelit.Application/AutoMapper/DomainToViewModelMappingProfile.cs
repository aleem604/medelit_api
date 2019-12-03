using AutoMapper;
using Medelit.Domain.Models;

namespace Medelit.Application.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Lead, LeadViewModel>();
            //CreateMap<LocationCategoryRelation, LocationCategoryRelationViewModel>();
            //CreateMap<LocationEntityRelation, LocationEntityRelationViewModel>();
            //CreateMap<EntityAttributeRelation, EntityAttributeRelationViewModel>();

            // entity 
            //CreateMap<Entity, OldEntityViewModel>();

            // entity contact
            //CreateMap<EntityContact, BusinessContactViewModel>();

            //// Business Offers
            //CreateMap<BusinessOffer, BusinessOffersViewModel>();


            //// profile models
            //CreateMap<ProfileAttribute, ProfileAttributeViewModel>();
            //CreateMap<ProfileReview, ProfileReviewViewModel>();
            //CreateMap<ProfileSection, ProfileSectionViewModel>();

            //CreateMap<LocationGallary, LocationGallaryVM>();
        }
    }
}
