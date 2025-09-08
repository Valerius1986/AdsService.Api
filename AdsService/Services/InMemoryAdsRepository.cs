using AdsService.Services.Interfaces;

namespace AdsService.Services
{
    public class InMemoryAdsRepository : IAdsRepository
    {
        private readonly Dictionary<string, List<string>> _adsByLocation = new();
        private readonly ReaderWriterLockSlim _lock = new();

        public void LoadData(Stream stream)
        {
            _lock.EnterWriteLock();
            try
            {
                _adsByLocation.Clear();
                using var reader = new StreamReader(stream);

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    ProcessLine(line);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public List<string> FindAds(string location)
        {
            _lock.EnterReadLock();
            try
            {
                var result = new HashSet<string>();
                FindAdsRecursive(location, result);
                return result.ToList();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        private void ProcessLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return;

            var parts = line.Split(':', 2);
            if (parts.Length != 2) return;

            var adName = parts[0].Trim();
            var locations = parts[1].Split(',')
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrEmpty(l))
                .ToList();

            foreach (var location in locations)
            {
                if (!_adsByLocation.ContainsKey(location))
                {
                    _adsByLocation[location] = new List<string>();
                }
                _adsByLocation[location].Add(adName);
            }
        }

        private void FindAdsRecursive(string location, HashSet<string> result)
        {
            if (_adsByLocation.TryGetValue(location, out var ads))
            {
                foreach (var ad in ads)
                {
                    result.Add(ad);
                }
            }

            var parentLocation = GetParentLocation(location);
            if (parentLocation != null)
            {
                FindAdsRecursive(parentLocation, result);
            }
        }

        private static string GetParentLocation(string location)
        {
            var lastSlashIndex = location.LastIndexOf('/');
            if (lastSlashIndex <= 0) return null;

            return location[..lastSlashIndex];
        }
    }
}
