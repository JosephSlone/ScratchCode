using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Security.Cryptography;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.Graphics.Imaging;

namespace Images
{
    public static class ImageHelper
    {
        public static async Task<WriteableBitmap> TestImage()
        {

            // example base64-encoded icon in PNG format at 24x24, 32 bit color
            string base64str = "iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAABGdBTUEAALGPC/xhBQAAABl0RVh0U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAAONSURBVEhLpVZZaxNRFJ6f4KNb3brZ1XSxrVq17pnpZDJpUxdUDIqgFKQPKhYF2xcXpBb6JPpQSDKZcY1L7YZYFbGKhfjkm/jkc37C8ftuMhBLG6098JHh3nO+s9xz7o1WTFbG9RWr4npsfaJztDJpzdQ6YQV+b0yYw9iz86pLExKvTRijm5OWND88LNHxC3L98125Mjsk/cDljzelB2twJjVOOLsu0TlAm7x5cYGiXZ4MZSNj52Ukc0/mfr0rittzI9Lx9AQd/VwVNwJ5moVlddyINXjdMjh7U6Z/PFkS+t5dlcZUJFuSWMQJ64nayq0vN+TR9/v/hYvv+6UJThDon05Qlg0sizV2Rh58u7MstDzukS2OnclT56QEXQIH0jvTK0Nfry0LRydPS7PbJaXJUEyRM3qWZl1Sl963Z2Xw04Vl4ejkcalMmXTyUzlA7fuq0W6bXV1OTh+TSx9OLwvd41Gp8nTZiixQmQ4NA5Mud0zZ8kiXQy9NlOnIf+Pc2yOy54UhAXA1uhGpSloDWrVjZUpQnqbHOjZ1sSdMib2JLBmnAHPcUBzNT3Spcy0JpOy0hgGR9Q5SwuJebB4aC0p40pCeaVMO/yOiQGhSl+DroOJoAVfAC0uL2zWjcdw3zHPQORGEAR0FxZ5CVkVgQYe6tFEOXuYcNPgOkEGGHdSERaZX6ICwpuBoEXDP16MNbcnBYOtRIgxdWitLhtKVOGQezO7nuhx4FRRjPCgmyfPGC5ETKgCAurQ5CAfkaMB5NnsRqXPsAW1N3OirT9lSjdba8UyX/a9ytSzMQhEVEBdGTvjloW07OGrAtQ13WkXSCqhB88+Bp9+BFBmJjoh8J34mC4E61KWNX54q15QdXjQ3aJRNCXO0NhVWWWxHBDyoQicswXxirvnkrP0+2NC25iF+vajUO3buqqDwsajDw1HqdKqBY6l8J0yd9SVZIbjGPeqQvD2N1oQt+7/N7f7zsqOsjRs2BkNKHUM5YTRMmXUlCaMkIcFvrnGPOgyI5LVuSHZ6PdnSRGjhNwFvb6whFZEyZMJUWU9G1oHO4IwwKwV8c4177Pla6DLyXSBH2xd/1fKZZHmXVKQMZdyItiNR29Mc+M017lW6hrShY3ComUUjny88E7TYcKvbnW3xujA0YZX+ppSuwGug0bOlFcTtuagH8qZLEzrCbRvDCzXMsUcJFFrxjSyH8Uj95W+Lpv0GtNxNNKdkcLwAAAAASUVORK5CYII=";

            // First load the image from the base64 string
            var img = await LoadImageFromBase64(base64str);

            // Now test round-trip Base64 to Base64
            string sTest = await SaveImageToBase64(img);

            img = await LoadImageFromBase64(sTest);

            // now save the round-trip image
            var accTok = await SaveImageToFile("checkbox-round-trip.png", img);

            // now load, round-trip, from disk.
            img = await LoadImageFromFile(accTok, true);

            return img;
        }


        public static async Task<WriteableBitmap> LoadImageFromBase64(string base64str)
        {
            try
            {
                var decodedBin = CryptographicBuffer.DecodeFromBase64String(base64str);

                var readStream = new InMemoryRandomAccessStream();

                await readStream.WriteAsync(decodedBin);
                readStream.Seek(0);

                var decoder = await BitmapDecoder.CreateAsync(readStream);

                var workBmp = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                readStream.Seek(0);

                await workBmp.SetSourceAsync(readStream);
                return workBmp;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<string> SaveImageToBase64(WriteableBitmap workBmp)
        {
            try
            {
                var writeStream = new InMemoryRandomAccessStream();
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, writeStream);

                // saving to a straight 32 bpp PNG, the dpiX and dpiY values are irrelevant, but also cannot be zero. 
                // We will use the default "since-the-beginning-of-time" 96 dpi.
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)workBmp.PixelWidth, (uint)workBmp.PixelHeight, 96, 96, workBmp.PixelBuffer.ToArray());

                await encoder.FlushAsync();
                writeStream.Seek(0);

                var proxyStream = writeStream.AsStreamForRead();
                byte[] b = new byte[proxyStream.Length];

                await proxyStream.ReadAsync(b, 0, (int)proxyStream.Length);

                proxyStream.Dispose();

                return (string)CryptographicBuffer.EncodeToBase64String(b.AsBuffer());
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<WriteableBitmap> LoadImageFromFile(string fileName, bool isToken = false)
        {
            try
            {
                var xc = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList;
                StorageFile decodedBin;

                if (!isToken)
                {
                    decodedBin = await StorageFile.GetFileFromPathAsync(fileName);
                }
                else
                {
                    decodedBin = await xc.GetFileAsync(fileName, Windows.Storage.AccessCache.AccessCacheOptions.None);
                }

                var readStream = await decodedBin.OpenReadAsync();

                var decoder = await BitmapDecoder.CreateAsync(readStream);
                var workBmp = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);

                readStream.Seek(0);

                await workBmp.SetSourceAsync(readStream);

                readStream.Dispose();
                return workBmp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<string> SaveImageToFile(string fileName, WriteableBitmap workBmp)
        {
            try
            {
                var writeStream = new InMemoryRandomAccessStream();
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, writeStream);

                // saving to a straight 32 bpp PNG, the dpiX and dpiY values are irrelevant, but also cannot be zero. 
                // We will use the default "since-the-beginning-of-time" 96 dpi.
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)workBmp.PixelWidth, (uint)workBmp.PixelHeight, 96, 96, workBmp.PixelBuffer.ToArray());

                await encoder.FlushAsync();

                writeStream.Seek(0);

                var dl = await DownloadsFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
                var imgFileOut = await dl.OpenStreamForWriteAsync();

                var fileProxyStream = writeStream.AsStreamForRead();

                await fileProxyStream.CopyToAsync(imgFileOut);
                await imgFileOut.FlushAsync();

                fileProxyStream.Dispose();
                imgFileOut.Dispose();

                var xc = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList;

                return xc.Add(dl);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
