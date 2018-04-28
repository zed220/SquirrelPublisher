using SquirrelPublisher.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SquirrelPublisher {
    public class MainViewModel {
        public MainViewModel() {
            UpdateExeCommand = new DelegateCommand(UpdateExeFilePath);
            UpdateSqurrelExeCommand = new DelegateCommand(UpdateSqurrelExeFilePath);
            PublishCommand = new DelegateCommand(Publish);
        }

        public Config Config { get; } = Config.LoadOrDefault();

        public ICommand UpdateExeCommand { get; }
        public ICommand UpdateSqurrelExeCommand { get; }
        public ICommand PublishCommand { get; }

        public void Save() {
            Config.Save();
        }

        public void UpdateExeFilePath() {
            Config.UpdateExeFilePath();
        }
        public void UpdateSqurrelExeFilePath() {
            Config.UpdateSquirrelExeFilePath();
        }
        public void Publish() {
            Config.Save();
            Config.BuildPackage();
            Config.BuildRelease();
            Config.Publish();
        }
    }

    class DelegateCommand : ICommand {
        readonly Action action;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action action) {
            this.action = action;
        }

        void ICommand.Execute(object arg) {
            action();
        }

        bool ICommand.CanExecute(object arg) {
            return true;
        }
    }
}
