using System;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// The namespace used for models.
/// </summary>
namespace GitEditor.Model
{
    /// <summary>
    /// The object representing the user credentials.
    /// Used to save user username and email in an XML file by the XMLManager.
    /// </summary>
    [Serializable]
    public class Credentials
    {
        /// <summary>
        /// The user username.
        /// </summary>
        [XmlAttribute("username")]
        public string username = "";

        /// <summary>
        /// The user email.
        /// </summary>
        [XmlAttribute("email")]
        public string email = ""; 
    }
}