using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace NetMetrixReporter
{
    public class CookieStore
    {
        private readonly object syncRoot = new object();

        public void Save(string key, string value)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (value == null) throw new ArgumentNullException("value");

            lock (syncRoot)
            {
                using (var storageFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (var stream = new IsolatedStorageFileStream(key, FileMode.Create, storageFile))
                    {
                        using (var writer = new StreamWriter(stream))
                        {
                            writer.WriteLine(value);
                        }
                    }
                }
            }
        }

        public string Load(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            string value;
            try
            {
                lock (syncRoot)
                {
                    using (var storageFile = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var stream = new IsolatedStorageFileStream(key, FileMode.OpenOrCreate, storageFile))
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                value = reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }

            return value;
        }
    }
}
