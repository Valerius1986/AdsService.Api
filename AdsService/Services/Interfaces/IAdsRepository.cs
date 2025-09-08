namespace AdsService.Services.Interfaces
{
    public interface IAdsRepository
    {
        void LoadData(Stream stream);
        List<string> FindAds(string location);
    }
}
