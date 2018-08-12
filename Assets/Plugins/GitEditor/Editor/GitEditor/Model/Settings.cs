using System.Xml.Serialization;

/// <summary>
/// The namespace used for models.
/// </summary>
namespace GitEditor.Model
{
    /// <summary>
    /// The object holding the settings.
    /// Used by XMLManager to save the settings.
    /// </summary>
    [XmlRoot("Settings")]
    public class Settings
    {
        /// <summary>
        /// The user credentials.
        /// </summary>
        [XmlElement("Credentials")]
        public Credentials credentials;

        public Settings()
        {
            credentials = new Credentials();
        }
    }
}