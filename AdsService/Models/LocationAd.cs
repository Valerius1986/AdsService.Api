namespace AdsService.Models
{
    public class LocationAd
    {
        public string Name { get; set; }
        public List<string> Locations { get; set; }

        public LocationAd(string name, List<string> locations)
        {
            Name = name;
            Locations = locations;
        }
    }
}
