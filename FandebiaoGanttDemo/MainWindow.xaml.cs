using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Windows;

namespace FandebiaoGanttDemo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadJson();
        }

        private List<p_city> CitysInJson;
        private List<p_task> TasksInJson;
        private Chart _mChart = null;
        ProjectManager _mManager = null;
        System.Windows.Forms.Integration.WindowsFormsHost host = null;

        private ProjectManager FillData(Chart _mChart, ProjectManager _mManager)
        {
            foreach (var task in TasksInJson)
            {
                var TaskFormat1 = new TaskFormat()
                {
                    Color = System.Drawing.Brushes.Black,
                    Border = Pens.Maroon,
                    BackFill = new SolidBrush(System.Drawing.Color.FromName(task.color)),
                    ForeFill = System.Drawing.Brushes.YellowGreen,
                    SlackFill = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.LightDownwardDiagonal, System.Drawing.Color.Blue, System.Drawing.Color.Transparent)
                };

                var city = CitysInJson.FirstOrDefault(s => s.id == task.parent);
                var newTask = new MyTask(_mManager) { id = task.id, Name = task.text, TaskFormat = TaskFormat1, City = city.text };
                _mManager.Add(newTask);

                var start = DateTime.Parse(task.start_date);
                var beginPoint = _mManager.Start;
                var startTS = start.Subtract(beginPoint);
                newTask.Start = startTS;
                newTask.Duration = DateTime.Parse(task.end_date).Subtract(start);
            }

            _mChart.Init(_mManager);
            _mChart.CreateTaskDelegate = delegate () { return new MyTask(_mManager); };
            _mChart.AllowTaskDragDrop = true;

            foreach (var task in _mManager.Tasks)
            {
                _mChart.SetToolTip(task);
            }

            var span = DateTime.Now - _mManager.Start;
            _mManager.Now = span; // set the "Now" marker at the correct date
            return _mManager;
        }

        private void Parse(string json)
        {
            CitysInJson = new List<p_city>();
            TasksInJson = new List<p_task>();

            dynamic JsonObject = JsonConvert.DeserializeObject<ExpandoObject>(json, new ExpandoObjectConverter());

            foreach (var item in JsonObject.data)
            {
                if (item.start_date == "")
                {
                    var e = new p_city();
                    e.id = item.id;
                    e.text = item.text;
                    e.start_date = "";
                    e.duration = 1;
                    e.parent = 0;
                    e.open = true;
                    e.render = "split";
                    CitysInJson.Add(e);
                }
                else
                {
                    var t = new p_task();
                    t.id = item.id;
                    t.text = item.text;
                    t.start_date = item.start_date;
                    t.end_date = item.end_date;
                    t.text = item.text;
                    t.parent = item.parent;
                    t.color = item.color;
                    TasksInJson.Add(t);
                }
            }
        }

        public void LoadJson()
        {
            var json = File.ReadAllText("data.json");
            Parse(json);
            host = new System.Windows.Forms.Integration.WindowsFormsHost();

            _mChart = new Chart(TimeResolution.Week);
            host.Child = _mChart;
            grid.Children.Add(host);

            var citys = CitysInJson.Select(s => s.text.ToString()).ToList();
            _mManager = new ProjectManager(citys);
            FillData(_mChart, _mManager);
        }

        public void SaveJson()
        {
            foreach (var item in _mManager.Citys)
            {
                //todo insert into DB
            }
        }

    }

    #region custom task and resource
    /// <summary>
    /// A custom resource of your own type (optional)
    /// </summary>
    [Serializable]
    public class MyResource
    {
        public string Name { get; set; }
    }
    /// <summary>
    /// A custom task of your own type deriving from the Task interface (optional)
    /// </summary>
    [Serializable]
    public class MyTask : TaskBase
    {
        public MyTask(ProjectManager manager)
            : base()
        {
            Manager = manager;
        }

        private ProjectManager Manager { get; set; }

        public new TimeSpan Start { get { return base.Start; } set { Manager.SetStart(this, value); } }
        public new TimeSpan End { get { return base.End; } set { Manager.SetEnd(this, value); } }
        public new TimeSpan Duration { get { return base.Duration; } set { Manager.SetDuration(this, value); } }
        public new float Complete { get { return base.Complete; } set { Manager.SetComplete(this, value); } }
    }
    #endregion custom task and resource

    public class p_city
    {
        public long id { get; set; }
        public string text { get; set; }
        public string start_date { get; set; }
        public long duration { get; set; }
        public long parent { get; set; }
        public bool open { get; set; }
        public string render { get; set; }
    }

    public class p_task
    {
        public long id { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string text { get; set; }
        public long parent { get; set; }
        public string color { get; set; }
    }

}
