using System;
using System.Collections.Generic;
using System.Windows;

namespace FandebiaoGanttDemo
{
    public partial class EditWindow: Window
    {
        public EditWindow()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as EditViewModel;
            vm.Save();
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class EditViewModel
    {
        public event Action<EditViewModel> Close;
        
        public EditViewModel()
        {

        }
        
        public void Init()
        {

        }
        public bool CanInit() => true;

        public void Save()
        {
            Close?.Invoke(this);
        }
        public bool CanSave() => true;

        public void Cancel()
        {

        }
        public bool CanCancel() => true;
        
        public virtual DateTime StartDateTime { get; set; }
        public virtual DateTime EndDateTime { get; set; }
        public virtual List<string> Citys { get; set; }
        public virtual string City { get; set; }
        public virtual string Covid_19 { get; set; }

    }

}
