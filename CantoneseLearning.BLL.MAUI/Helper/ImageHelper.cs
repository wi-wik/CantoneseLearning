using viwik.CantoneseLearning.BLL.MAUI.Manager;
using viwik.CantoneseLearning.Model;

namespace viwik.CantoneseLearning.BLL.MAUI.Helper
{
    public class ImageHelper
    {
        public static bool IsImagehubImage(string url)
        {
            return url?.ToLower().Contains("imagehub.cc") == true;
        }

        public static async Task<string> DecorateImageUrl(V_CantoneseMedia media, bool awaitDownladed = false)
        {
            if (media == null)
            {
                return null;
            }

            string imageUrl = media.ImageUrl;

            if (imageUrl == null)
            {
                return null;
            }

            string cacheFilePath = CacheManager.GetMediaImageCacheFilePath(media);

            if (File.Exists(cacheFilePath))
            {
                return cacheFilePath;
            }

            if (!awaitDownladed)
            {
                DownloadImage(media, cacheFilePath);
            }
            else
            {
                return await DownloadImage(media, cacheFilePath);
            }


            if (!IsImagehubImage(imageUrl))
            {
                return imageUrl;
            }

            return null;
        }

        private static async Task<string> DownloadImage(V_CantoneseMedia media, string cacheFilePath)
        {
            try
            {
                string imageUrl = media.ImageUrl;

                using (var handler = new HttpClientHandler())
                {
                    var client = new HttpClient(handler);

                    if (IsImagehubImage(imageUrl))
                    {
                        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                    }

                    var response = await client.GetAsync(imageUrl);

                    var data = await response.Content.ReadAsByteArrayAsync();

                    File.WriteAllBytes(cacheFilePath, data);

                    media.ImageUrl = cacheFilePath;

                    return cacheFilePath;
                }

            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public static async Task<string> GetImageUrl(V_CantoneseMedia media)
        {
            ImageSource source = await DecorateImageUrl(media, true);

            byte[] data = null;

            try
            {
                if (source is UriImageSource us)
                {
                    return us.Uri.ToString();
                }
                else if (source is FileImageSource fs)
                {
                    data = File.ReadAllBytes(fs.File);
                }
                else if (source is StreamImageSource ss)
                {
                    var func = ss.Stream; ;

                    using (Stream stream = await func(CancellationToken.None))
                    {
                        BinaryReader reader = new BinaryReader(stream);

                        data = reader.ReadBytes((int)stream.Length);
                    }
                }

                if (data != null)
                {
                    return $"data:image/jpeg;base64,{Convert.ToBase64String(data)}";
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public static async Task<IEnumerable<T>> DecorateMedias<T>(IEnumerable<T> medias)
          where T : V_CantoneseMedia
        {
            foreach (var media in medias)
            {
                string imageUrl = media.ImageUrl;

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    media.ImageUrl = await DecorateImageUrl(media);
                }
            }

            return medias;
        }
    }
}
