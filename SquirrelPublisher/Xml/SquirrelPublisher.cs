using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace SquirrelPublisher.Xml {
    [XmlRoot]
    public class Config : INotifyPropertyChanged {
        static string[] SupportedFiles = new string[] { ".exe", ".dll", ".xml" };

        static string ConfigPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SquirrelPublisher.conf");

        string nuspecDirectory;
        string nuspecShortName;
        string previousNuspecFullPath;
        string sourceFolderName;
        string targetNuspecFolderName = @"lib\net45";
        string sourceExeFilePath;
        string squirrelExeFilePath;
        Nuspec nuspec;


        [XmlAttribute]
        public string NuspecDirectory {
            get { return nuspecDirectory; }
            set {
                if(nuspecDirectory == value)
                    return;
                nuspecDirectory = value;
                PropChanged(nameof(NuspecDirectory));
            }
        }
        [XmlAttribute]
        public string NuspecShortName {
            get { return nuspecShortName; }
            set {
                if(nuspecShortName == value)
                    return;
                nuspecShortName = value;
                PropChanged(nameof(NuspecShortName));
            }
        }
        [XmlAttribute]
        public string PreviousNuspecFullPath {
            get { return previousNuspecFullPath; }
            set {
                if(previousNuspecFullPath == value)
                    return;
                previousNuspecFullPath = value;
                PropChanged(nameof(PreviousNuspecFullPath));
            }
        }

        [XmlAttribute]
        public string SourceFolderName {
            get { return sourceFolderName; }
            set {
                if(sourceFolderName == value)
                    return;
                sourceFolderName = value;
                PropChanged(nameof(SourceFolderName));
            }
        }

        [XmlIgnore]
        public string TargetNuspecFolderName {
            get { return targetNuspecFolderName; }
            set {
                if(targetNuspecFolderName == value)
                    return;
                targetNuspecFolderName = value;
                PropChanged(nameof(TargetNuspecFolderName));
            }
        }

        [XmlAttribute]
        public string SourceExeFilePath {
            get { return sourceExeFilePath; }
            set {
                if(sourceExeFilePath == value)
                    return;
                sourceExeFilePath = value;
                PropChanged(nameof(SourceExeFilePath));
            }
        }
        [XmlAttribute]
        public string SquirrelExeFilePath {
            get { return squirrelExeFilePath; }
            set {
                if(squirrelExeFilePath == value)
                    return;
                squirrelExeFilePath = value;
                PropChanged(nameof(SquirrelExeFilePath));
            }
        }
        [XmlIgnore]
        public Nuspec Nuspec {
            get { return nuspec; }
            set {
                if(nuspec == value)
                    return;
                nuspec = value;
                PropChanged(nameof(Nuspec));
            }
        }

        public void UpdateExeFilePath() {
            if(TryOpenFile("Applications (*.exe)|*.exe", out string fileName)) {
                SourceExeFilePath = fileName;
                if(String.IsNullOrEmpty(NuspecShortName)) {

                    NuspecShortName = Nuspec.Metadata.Id = Nuspec.Metadata.Title = Path.GetFileNameWithoutExtension(fileName);
                }
                Actualize();
            }
        }

        void Actualize() {
            UpdateNuspecVersion();
            NuspecDirectory = Path.GetDirectoryName(SourceExeFilePath);
        }

        public void UpdateSquirrelExeFilePath() {
            if(TryOpenFile("Applications (*.exe)|*.exe", out string fileName))
                SquirrelExeFilePath = fileName;
        }

        void PopulateNuspecFiles() {
            foreach(var file in Directory.GetFiles(NuspecDirectory)) {
                if(SupportedFiles.Contains(Path.GetExtension(file)))
                    Nuspec.Files.Add(new FileInfo() { Src = file, Target = Path.Combine(TargetNuspecFolderName, Path.GetFileName(file)) });
            }
        }

        bool TryOpenFile(string filter, out string fileName) {
            fileName = null;
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = filter, Multiselect = false };
            if(!openFileDialog.ShowDialog().GetValueOrDefault(false))
                return false;
            if(!File.Exists(openFileDialog.FileName)) {
                MessageBox.Show("File not exist: " + openFileDialog.FileName);
                return false;
            }
            fileName = openFileDialog.FileName;
            return true;
        }

        void UpdateNuspecVersion() {
            if(String.IsNullOrEmpty(SourceExeFilePath) || !File.Exists(SourceExeFilePath))
                return;
            Nuspec.Metadata.Version = FileVersionInfo.GetVersionInfo(SourceExeFilePath).ProductVersion;
        }

        public static Config LoadOrDefault() {
            Config config = LoadCore();
            if(String.IsNullOrEmpty(config.PreviousNuspecFullPath) || !File.Exists(config.PreviousNuspecFullPath)) {
                config.Nuspec = new Nuspec();
            }
            else {
                config.Nuspec = SerializationHelper.Deserialize<Nuspec>(config.previousNuspecFullPath);
                config.Actualize();
            }
            return config;
        }

        static Config LoadCore() {
            if(!File.Exists(ConfigPath))
                return new Config();
            return SerializationHelper.Deserialize<Config>(ConfigPath);
        }

        public void Save() {
            PopulateNuspecFiles();
            string nuspecTargetFolder = Path.GetDirectoryName(SquirrelExeFilePath);
            string targetNuspecFileName = Path.Combine(nuspecTargetFolder, NuspecShortName + "." + nuspec.Metadata.Version + ".nuspec");
            SerializationHelper.Serialize(targetNuspecFileName, Nuspec);
            PreviousNuspecFullPath = targetNuspecFileName;
            SerializationHelper.Serialize(ConfigPath, this);
        }
        public void BuildPackage() {
            //download nuget
            RunProcess("Nuget.exe", "pack " + PreviousNuspecFullPath);

        }
        public void BuildSquirrelRelease() {
            RunProcess(SquirrelExeFilePath, "--releasify " + Path.ChangeExtension(PreviousNuspecFullPath, ".nupkg"));
        }
        public void Publish() {
            foreach(var file in Directory.EnumerateFiles(Path.Combine(Path.GetDirectoryName(squirrelExeFilePath), "Releases"))) {
                string targetPath = Path.Combine(@"\\corp\internal\common\visualTests_squirrel\", Path.GetFileName(file));
                if(File.Exists(targetPath)) {
                    if(new System.IO.FileInfo(targetPath).Length == new System.IO.FileInfo(file).Length)
                        continue;
                }
                File.Copy(file, targetPath, true);
            }
        }

        void RunProcess(string fileName, string arguments) {
            string prevDir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(Path.GetDirectoryName(PreviousNuspecFullPath));
            Process runnerProcess = new Process();
            runnerProcess.StartInfo.FileName = fileName;
            runnerProcess.StartInfo.Arguments = arguments;
            runnerProcess.StartInfo.CreateNoWindow = true;
            runnerProcess.StartInfo.UseShellExecute = false;
            runnerProcess.StartInfo.RedirectStandardOutput = true;
            runnerProcess.Start();
            runnerProcess.WaitForExit();
            Directory.SetCurrentDirectory(prevDir);
        }

        void PropChanged(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
