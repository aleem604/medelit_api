namespace Medelit.Application
{
  public  class ServiceProfessionalRelationVeiwModel
    {
        public long Id { get; set; }
        public long ServiceId { get; set; }
        public ServiceViewModel Service { get; set; }
        public long ProfessionalId { get; set; }
        public ProfessionalViewModel Professional { get; set; }
    }
}
