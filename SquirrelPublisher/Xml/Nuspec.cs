using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SquirrelPublisher.Xml {
    [XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd", ElementName = "package", DataType = "string", IsNullable = true)]
    public class Nuspec {
        [XmlElement("metadata")]
        public Metadata Metadata { get; set; } = new Metadata();

        [XmlArray("files")]
        [XmlArrayItem("file")]
        public List<FileInfo> Files { get; set; } = new List<FileInfo>();
    }

    [XmlType("metadata")]
    public class Metadata : INotifyPropertyChanged {
        string id;
        [XmlElement("id")]
        public string Id {
            get {
                return id;
            }
            set {
                if(id == value) {
                    return;
                }

                id = value;
                PropChanged(nameof(Id));
            }
        }

        string version;
        [XmlElement("version")]
        public string Version {
            get {
                return version;
            }
            set {
                if(version == value) {
                    return;
                }

                version = value;
                PropChanged(nameof(Version));
            }
        }
        string title;
        [XmlElement("title")]
        public string Title {
            get {
                return title;
            }
            set {
                if(title == value) {
                    return;
                }

                title = value;
                PropChanged(nameof(Title));
            }
        }
        string authors = "Empty";
        [XmlElement("authors")]
        public string Authors {
            get {
                return authors;
            }
            set {
                if(authors == value) {
                    return;
                }

                authors = value;
                PropChanged(nameof(Authors));
            }
        }
        bool requireLicenseAcceptance;
        [XmlElement("requireLicenseAcceptance")]
        public bool RequireLicenseAcceptance {
            get {
                return requireLicenseAcceptance;
            }
            set {
                if(requireLicenseAcceptance == value) {
                    return;
                }

                requireLicenseAcceptance = value;
                PropChanged(nameof(RequireLicenseAcceptance));
            }
        }
        string description;
        [XmlElement("description")]
        public string Description {
            get {
                return description;
            }
            set {
                if(description == value) {
                    return;
                }

                description = value;
                PropChanged(nameof(Description));
            }
        }

        void PropChanged(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    [XmlType("file")]
    public class FileInfo {
        [XmlAttribute("src")]
        public string Src { get; set; }
        [XmlAttribute("target")]
        public string Target { get; set; }
    }
}
