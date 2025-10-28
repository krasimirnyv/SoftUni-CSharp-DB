using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace MusicHub
{
    using System;

    using Data;
    using Initializer;
    using ViewModel;

    public class StartUp
    {
        private static readonly Func<MusicHubDbContext, int, IEnumerable<AlbumViewModel>> _compiledAlbumQuery =
            EF.CompileQuery((MusicHubDbContext context, int producerId) =>
                context
                    .Albums
                    .Where(a => a.ProducerId == producerId)
                    .Select(a => new AlbumViewModel
                    {
                        Name = a.Name,
                        ReleaseDate = a.ReleaseDate,
                        ProducerName = a.Producer!.Name,
                        AlbumSongs = a.Songs
                            .Select(s => new SongViewModel
                            {
                                Name = s.Name,
                                Price = s.Price,
                                WriterName = s.Writer.Name
                            })
                            .OrderByDescending(s => s.Name)
                            .ThenBy(s => s.WriterName)
                            .ToArray(),
                        TotalPrice = a.Price
                    }));

        private static readonly Func<MusicHubDbContext, int, IEnumerable<SongViewModel>> _compiledSongQuery =
            EF.CompileQuery((MusicHubDbContext context, int duration) =>
                context
                    .Songs
                    .Select(s => new SongViewModel
                    {
                        Name = s.Name,
                        PerformersNames = s.SongPerformers
                            .Select(sp => new PerformerViewModel
                            {
                                FirstName = sp.Performer.FirstName,
                                LastName = sp.Performer.LastName
                            })
                            .OrderBy(sp => sp.FirstName)
                            .ThenBy(sp => sp.LastName)
                            .ToArray(),
                        WriterName = s.Writer.Name,
                        AlbumProducerName =
                            s.Album != null
                                ? (s.Album.Producer != null ? s.Album.Producer.Name : null)
                                : null,
                        Duration = s.Duration
                    }));
            
        public static void Main()
        {
            using MusicHubDbContext context = new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            // Printing results
            Console.WriteLine(ExportAlbumsInfo(context, 9));
            Console.WriteLine("###############################################");
            Console.WriteLine(ExportSongsAboveDuration(context, 4));
            
            // Testing speed between compiled and non-compiled queries
            Stopwatch sw = new Stopwatch();
            string testResult = string.Empty;
            
            sw.Start();
            testResult = ExportAlbumsInfo(context, 9); // 365ms
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);

            sw.Restart();
            testResult = ExportAlbumsInfo_Compiled(context, 9); // 30ms (EF Core Cache + PreCompile)
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            
            sw.Restart();
            testResult = ExportSongsAboveDuration(context, 4); // 3ms
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            
            sw.Restart();
            testResult = ExportSongsAboveDuration_Compiled(context, 4); // 32 ms - now is slower because of its structure (in method 'ExportSongsAboveDuration' ordering)
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            // Always prefer to use parameterized queries to avoid SQL Injection attacks and increase EF Cache performance during SQL generation
            // LINQ using Constant value can't make use of the EF Core Cache and disturb the Cache Memory -> SQL Generation Overhead
            
            var albums = context
                .Albums
                .Where(a => a.ProducerId == producerId)
                .Select(a => new
                {
                    a.Name,
                    a.ReleaseDate,
                    ProducerName = a.Producer!.Name,
                    AlbumSongs = a.Songs
                        .Select(s => new
                        {
                            s.Name,
                            s.Price,
                            WriterName = s.Writer.Name
                        })
                        .OrderByDescending(s => s.Name)
                        .ThenBy(s => s.WriterName)
                        .ToArray(),
                    TotalPrice = a.Price
                })
                .ToArray()
                .OrderByDescending(a => a.TotalPrice)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var album in albums)
            {
                sb
                    .AppendLine($"-AlbumName: {album.Name}")
                    .AppendLine($"-ReleaseDate: {album.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}")
                    .AppendLine($"-ProducerName: {album.ProducerName}")
                    .AppendLine("-Songs:");

                int songNumber = 1;
                foreach (var song in album.AlbumSongs)
                {
                    sb
                        .AppendLine($"---#{songNumber++}")
                        .AppendLine($"---SongName: {song.Name}")
                        .AppendLine($"---Price: {song.Price.ToString("F2")}")
                        .AppendLine($"---Writer: {song.WriterName}");
                }
                
                sb.AppendLine($"-AlbumPrice: {album.TotalPrice.ToString("F2")}");
            }
            
            return sb.ToString().TrimEnd();
        }
        
        public static string ExportAlbumsInfo_Compiled(MusicHubDbContext context, int producerId)
        {
            
            var albums = _compiledAlbumQuery(context, producerId)
                .ToArray()
                .OrderByDescending(a => a.TotalPrice)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var album in albums)
            {
                sb
                    .AppendLine($"-AlbumName: {album.Name}")
                    .AppendLine($"-ReleaseDate: {album.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}")
                    .AppendLine($"-ProducerName: {album.ProducerName}")
                    .AppendLine("-Songs:");

                int songNumber = 1;
                foreach (var song in album.AlbumSongs)
                {
                    sb
                        .AppendLine($"---#{songNumber++}")
                        .AppendLine($"---SongName: {song.Name}")
                        .AppendLine($"---Price: {song.Price.ToString("F2")}")
                        .AppendLine($"---Writer: {song.WriterName}");
                }
                
                sb.AppendLine($"-AlbumPrice: {album.TotalPrice.ToString("F2")}");
            }
            
            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songs = context
                .Songs
                .Select(s => new
                {
                    s.Name,
                    PerformersNames = s.SongPerformers
                        .Select(sp => new
                        {
                            sp.Performer.FirstName,
                            sp.Performer.LastName
                        })
                        .OrderBy(sp => sp.FirstName)
                        .ThenBy(sp => sp.LastName)
                        .ToArray(),
                    WriterName = s.Writer.Name,
                    AlbumProducerName = 
                        s.Album != null ?
                            (s.Album.Producer != null ? s.Album.Producer.Name : null) 
                            : null,
                    s.Duration
                })
                .OrderBy(s => s.Name)
                .ThenBy(s => s.WriterName)
                .ToArray()
                .Where(s => s.Duration.TotalSeconds > duration) // In-memory filtering due to limitations in EntityFrameworkCore.SqlServer DbProvider
                .ToArray();

            int songNumber = 1;
            StringBuilder sb = new StringBuilder();
            foreach (var song in songs)
            {
                sb
                    .AppendLine($"-Song #{songNumber++}")
                    .AppendLine($"---SongName: {song.Name}")
                    .AppendLine($"---Writer: {song.WriterName}");

                foreach (var performer in song.PerformersNames)
                {
                    sb.AppendLine($"---Performer: {performer.FirstName} {performer.LastName}");
                }

                sb
                    .AppendLine($"---AlbumProducer: {song.AlbumProducerName}")
                    .AppendLine($"---Duration: {song.Duration.ToString("c")}");
            }
            
            return sb.ToString().TrimEnd();
        }
        
         public static string ExportSongsAboveDuration_Compiled(MusicHubDbContext context, int duration)
        {
            var songs = _compiledSongQuery(context, duration)
                .OrderBy(s => s.Name)
                .ThenBy(s => s.WriterName)
                .ToArray()
                .Where(s => s.Duration.TotalSeconds > duration)
                .ToArray();

            int songNumber = 1;
            StringBuilder sb = new StringBuilder();
            foreach (var song in songs)
            {
                sb
                    .AppendLine($"-Song #{songNumber++}")
                    .AppendLine($"---SongName: {song.Name}")
                    .AppendLine($"---Writer: {song.WriterName}");

                foreach (var performer in song.PerformersNames)
                {
                    sb.AppendLine($"---Performer: {performer.FirstName} {performer.LastName}");
                }

                sb
                    .AppendLine($"---AlbumProducer: {song.AlbumProducerName}")
                    .AppendLine($"---Duration: {song.Duration.ToString("c")}");
            }
            
            return sb.ToString().TrimEnd();
        }
    }
}
