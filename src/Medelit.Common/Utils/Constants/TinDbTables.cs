using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Common
{
    public static class TinDbObjects
    {
        public const string ActiveSchema = "dbo";

        public static string BestOfTag = $"{ActiveSchema}.best_of_tag";
        public static string BusinessOffer = $"{ActiveSchema}.business_offer";
        public static string TinAttribute = $"{ActiveSchema}.tin_attribute";
        public static string TouristAttraction = $"{ActiveSchema}.tourist_attraction";

        /*Entity Objects*/

        public static string Entity = $"{ActiveSchema}.entity";
        public static string EntityGallary = $"{ActiveSchema}.entity_gallary";
        public static string EntityCategory = $"{ActiveSchema}.entity_category";
        public static string EntityAddress = $"{ActiveSchema}.entity_address";
        public static string EntityTouristType = $"{ActiveSchema}.entity_tourist_type";
        public static string EntityServesCuisine = $"{ActiveSchema}.entity_serves_cuisine";
        public static string EntitySocialNetwork = $"{ActiveSchema}.entity_social_network";
        public static string EntityTouristAttraction = $"{ActiveSchema}.entity_tourist_attraction";
        public static string EntityType = $"{ActiveSchema}.entity_type";
        public static string EntityOffer = $"{ActiveSchema}.entity_offer";
        public static string EntityAttribute = $"{ActiveSchema}.entity_attribute";
        public static string EntityAbout = $"{ActiveSchema}.entity_about";
        public static string EntityEvent = $"{ActiveSchema}.entity_event";
        public static string EntityClassification = $"{ActiveSchema}.entity_classification";
        public static string EntityFeedback = $"{ActiveSchema}.entity_feedback";
        public static string EntityComment = $"{ActiveSchema}.entity_comment";
        public static string EntityContact = $"{ActiveSchema}.entity_contact";
        

        // Classification
        public static string Classification = $"{ActiveSchema}.classification";
        public static string Level1Classification = $"{ActiveSchema}.level1_classification";
        public static string Level2Classification = $"{ActiveSchema}.level2_classification";
        public static string Level3Classification = $"{ActiveSchema}.level3_classification";

        // Category
        public static string Category = $"{ActiveSchema}.category";
        public static string Level1Category = $"{ActiveSchema}.level1_Category";
        public static string Level2Category = $"{ActiveSchema}.level2_Category";
        public static string Level3Category = $"{ActiveSchema}.level3_Category";

        // Location
        public static string Country = $"{ActiveSchema}.country";
        public static string Region = $"{ActiveSchema}.region";
        public static string City = $"{ActiveSchema}.city";
        public static string Neighborhood = $"{ActiveSchema}.neighbourhood";
        public static string LocationAbout = $"{ActiveSchema}.location_about";
        public static string LocationAttraction = $"{ActiveSchema}.location_attraction";
        public static string LocationOffer = $"{ActiveSchema}.location_offer";
        public static string LocationFeaturedCategory = $"{ActiveSchema}.location_featured_category";
        public static string LocationFeaturedEntity = $"{ActiveSchema}.location_featured_entity";
        public static string LocationFeaturedEvent = $"{ActiveSchema}.location_featured_event";
        public static string LocationFeaturedLocations = $"{ActiveSchema}.location_featured_locations";
        public static string LocationGallary = $"{ActiveSchema}.location_gallary";


    }
}
