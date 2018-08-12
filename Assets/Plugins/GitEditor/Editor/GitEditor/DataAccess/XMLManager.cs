using System.IO;
using System.Xml.Serialization;

/// <summary>
/// The namespace used for Data Access classes.
/// </summary>
namespace GitEditor.DataAccess
{
    /// <summary>
    /// Manage XML read and write.
    /// Mainly used for settings.
    /// </summary>
    public class XMLManager
    {
        /// <summary>
        /// Read a DataType from a XML file.
        /// Used to read the settings from a previously written file.
        /// </summary>
        /// <typeparam name="DataType">The type of the object to read.</typeparam>
        /// <param name="filePath">The file path.</param>
        /// <returns>The read object.</returns>
        public static DataType ReadXML<DataType>(string filePath) where DataType : class, new()
        {
            if (!System.IO.File.Exists(filePath))
            {
                WriteXML(filePath, new DataType());
            }

            var serializer = new XmlSerializer(typeof(DataType));
            FileStream stream = null;
            DataType data = null;
            try
            {
                stream = new FileStream(filePath, FileMode.OpenOrCreate);
                data = serializer.Deserialize(stream) as DataType;

            }
            catch (System.Exception e)
            {

                throw e;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return (data == null ? new DataType() : data);
        }

        /// <summary>
        /// Writes an object to a XML file.
        /// Used to save the settings.
        /// </summary>
        /// <typeparam name="DataType">The type of the object to save.</typeparam>
        /// <param name="filePath">The file path.</param>
        /// <param name="data">The object to save.</param>
        public static void WriteXML<DataType>(string filePath, DataType data) where DataType : class
        {
            var serializer = new XmlSerializer(typeof(DataType));
            FileStream stream = null;
            try
            {
                stream = new FileStream(filePath, FileMode.Create);
                serializer.Serialize(stream, data);

            }
            catch (System.Exception e)
            {

                throw e;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }
    }
}