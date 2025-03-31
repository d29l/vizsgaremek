using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjektBackend.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CS8604

namespace ProjektBackend.Cleanup
{
    public class FileCleanupService : IHostedService, IDisposable
    {
        private readonly string _imageFolder;
        private readonly string _bannerFolder;
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer? _cleanupTimer;

        public FileCleanupService(IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _imageFolder = Path.Combine(Directory.GetCurrentDirectory(), configuration["Storage:ImageFolder"]);
            _bannerFolder = Path.Combine(Directory.GetCurrentDirectory(), configuration["Storage:BannerFolder"]);
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cleanupTimer = new Timer(RunCleanup, null, TimeSpan.Zero, TimeSpan.FromMinutes(30));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cleanupTimer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _cleanupTimer?.Dispose();
        }

        private async void RunCleanup(object? state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var scopedDb = scope.ServiceProvider.GetRequiredService<ProjektContext>();
                await DeleteUnusedFiles(scopedDb);
            }
        }

        private async Task DeleteUnusedFiles(ProjektContext db)
        {
            try
            {
                var usedImageUrls = await db.Profiles.Select(p => p.ProfilePicture).ToListAsync();
                var usedBannerUrls = await db.Profiles.Select(p => p.Banner).ToListAsync();

                var imageFiles = Directory.GetFiles(_imageFolder);
                var bannerFiles = Directory.GetFiles(_bannerFolder);

                foreach (var imageFile in imageFiles)
                {
                    var imageUrl = "/Storage/Images/" + Path.GetFileName(imageFile);
                    if (!usedImageUrls.Contains(imageUrl) && Path.GetFileName(imageFile) != "default.png")
                    {
                        File.Delete(imageFile);
                    }
                }

                foreach (var bannerFile in bannerFiles)
                {
                    var bannerUrl = "/Storage/Banners/" + Path.GetFileName(bannerFile);
                    if (!usedBannerUrls.Contains(bannerUrl) && Path.GetFileName(bannerFile) != "default_banner.png")
                    {
                        File.Delete(bannerFile);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during file cleanup: {ex.Message}");
            }
        }
    }
}