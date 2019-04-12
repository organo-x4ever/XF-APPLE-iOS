using System.Threading.Tasks;

namespace com.organo.x4ever.Services
{
    public interface ISecureStorage
    {
        /// <summary>
        /// Stores data.
        /// </summary>
        /// <param name="key">
        /// Key for the data.
        /// </param>
        /// <param name="dataBytes">
        /// Data bytes to store.
        /// </param>
        void Store(string key, string data);

        /// <summary>
        /// Stores data.
        /// </summary>
        /// <param name="key">
        /// Key for the data.
        /// </param>
        /// <param name="dataBytes">
        /// Data bytes to store.
        /// </param>
        void StoreByte(string key, byte[] data);

        /// <summary>
        /// Stores data.
        /// </summary>
        /// <param name="key">
        /// Key for the data.
        /// </param>
        /// <param name="dataBytes">
        /// Data bytes to store.
        /// </param>
        void StoreBytesFromString(string key, string data);

        /// <summary>
        /// Retrieves stored data.
        /// </summary>
        /// <param name="key">
        /// Key for the data.
        /// </param>
        /// <returns>
        /// Byte array of stored data.
        /// </returns>
        T Retrieve<T>(string key);

        /// <summary>
        /// Retrieves stored data.
        /// </summary>
        /// <param name="key">
        /// Key for the data.
        /// </param>
        /// <returns>
        /// Byte array of stored data.
        /// </returns>
        string RetrieveStringFromBytes(string key);

        /// <summary>
        /// Deletes data.
        /// </summary>
        /// <param name="key">
        /// Key for the data to be deleted.
        /// </param>
        void Delete(string key);
    }
}