﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Q42.WinRT.Data
{
    /// <summary>
    /// Can cache Uri data
    /// </summary>
    public static class WebDataCache
    {
        private static readonly string CacheFolder = "_webdatacache";

        /// <summary>
        /// Stores webdata in cache based on uri as key
        /// Returns file
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="forceGet"></param>
        /// <returns></returns>
        public async static Task<StorageFile> GetAsync(Uri uri, bool forceGet = false)
        {
            string key = uri.ToCacheKey();

            StorageFile file = null;

            //Try get the data from the cache
            var folder = await GetFolderAsync().ConfigureAwait(false);
            var exist = await folder.ContainsFileAsync(key).ConfigureAwait(false);

            //If file is not available or we want to force getting this file
            if (!exist || forceGet)
            {
                //else, load the data
              file = await SetAsync(uri).ConfigureAwait(false);
            }

            if (file == null)
              file = await folder.GetFileAsync(key);

            return file;
        }

        /// <summary>
        /// Stores webdata in cache based on uri as key
        /// Returns local uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async static Task<Uri> GetLocalUriAsync(Uri uri)
        {
          //Ignore these uri schemes
          if (uri.Scheme == "ms-resource"
              || uri.Scheme == "ms-appx"
              || uri.Scheme == "ms-appdata")
            return uri;

          string key = uri.ToCacheKey();

          //Try get the data from the cache
          var folder = await GetFolderAsync().ConfigureAwait(false);

          var contains = await folder.ContainsFileAsync(key).ConfigureAwait(false);
          if (!contains)
          {
            //else, load the data
            await SetAsync(uri).ConfigureAwait(false);
          }

          string localUri = string.Format("ms-appdata:///local/{0}/{1}", CacheFolder, key);

          return new Uri(localUri);

        }

        /// <summary>
        /// Get the cache folder to read/write
        /// </summary>
        /// <returns></returns>
        private static async Task<StorageFolder> GetFolderAsync()
        {
            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;

            if (!string.IsNullOrEmpty(CacheFolder))
            {
              folder = await folder.CreateFolderAsync(CacheFolder, CreationCollisionOption.OpenIfExists);
            }

            //StorageFolder.GetFolderFromPathAsync

            return folder;
        }

        /// <summary>
        /// Insert given uri in cache. Data will be loaded from the web
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static async Task<StorageFile> SetAsync(Uri uri)
        {
            string key = uri.ToCacheKey();

            var folder = await GetFolderAsync();

            HttpClient webClient = new HttpClient();
            var bytes = await webClient.GetByteArrayAsync(uri).ConfigureAwait(false);

            //Save data to cache
            var file = await folder.CreateFileAsync(key, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteBytesAsync(file, bytes);
            return file;
        }

        /// <summary>
        /// Delete from cache based on Uri (=key)
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static Task Delete(Uri uri)
        {
            return Task.Run(async () =>
                {
                  var file = await GetAsync(uri).ConfigureAwait(false);

                    await file.DeleteAsync();
                });
        }

        /// <summary>
        /// Clear the complete webcache
        /// </summary>
        /// <returns></returns>
        public static Task ClearAll()
        {
            return Task.Run(async () =>
            {
              var folder = await GetFolderAsync().ConfigureAwait(false);

                try
                {
                  await folder.DeleteAsync(StorageDeleteOption.PermanentDelete).AsTask().ConfigureAwait(false);
                }
                catch (UnauthorizedAccessException)
                {
                }
            });
        }
    }
}
