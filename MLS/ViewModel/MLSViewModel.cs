﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.NotifyBase;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using System.Threading;

namespace MLS.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class MLSViewModel : PropertyNotifyObject
    {
        public ObservableCollection<PlayerScore> SourcePlayers { get; set; }
        public ObservableCollection<PlayerScore> SourceFouls { get; set; }
        public ObservableCollection<TeamScore> SourceTeams { get; set; }

        private DelegateCommand _importExcelCommand = null;
        private DelegateCommand<ObservableCollection<TeamScore>> _exportExcelCommand = null;

        public MLSViewModel()
        {
            SourcePlayers = new ObservableCollection<PlayerScore>();
            SourceTeams = new ObservableCollection<TeamScore>();
            SourceFouls = new ObservableCollection<PlayerScore>();

            Teams = new ObservableCollection<TeamScore>();
            Players = new ObservableCollection<PlayerScore>();
            FoulPlayers = new ObservableCollection<PlayerScore>();

            LoadVisibilty = Visibility.Hidden;

            Tops = new string[] { "全部", "前3", "前8", "前10", "前20", "前50" };
            TabIndex = 0;
            TopSelect = Tops.First();
        }

        public int ErrorCount
        {
            get { return this.GetValue(s => s.ErrorCount); }
            set { this.SetValue(s => s.ErrorCount, value); }
        }

        public Visibility LoadVisibilty
        {
            get { return this.GetValue(s => s.LoadVisibilty); }
            set { this.SetValue(s => s.LoadVisibilty, value); }
        }

        public string[] Tops
        {
            get { return this.GetValue(s => s.Tops); }
            set { this.SetValue(s => s.Tops, value); }
        }

        public int TabIndex
        {
            get { return this.GetValue(s => s.TabIndex); }
            set { this.SetValue(s => s.TabIndex, value); }
        }

        public string TopSelect
        {
            get { return this.GetValue(s => s.TopSelect); }
            set { this.SetValue(s => s.TopSelect, value); }
        }

        public ObservableCollection<PlayerScore> Players
        {
            get { return this.GetValue(s => s.Players); }
            set { this.SetValue(s => s.Players, value); }
        }

        public ObservableCollection<TeamScore> Teams
        {
            get { return this.GetValue(s => s.Teams); }
            set { this.SetValue(s => s.Teams, value); }
        }

        public ObservableCollection<PlayerScore> FoulPlayers
        {
            get { return this.GetValue(s => s.FoulPlayers); }
            set { this.SetValue(s => s.FoulPlayers, value); }
        }

        public DelegateCommand ImportExcel
        {
            get
            {
                if (_importExcelCommand == null)
                {
                    _importExcelCommand = new DelegateCommand(ReadExcel, () => true);
                }
                return _importExcelCommand;
            }
        }

        public DelegateCommand<ObservableCollection<TeamScore>> ExportTeamScoreCommand
        {
            get
            {
                if (_exportExcelCommand == null)
                {
                    _exportExcelCommand = new DelegateCommand<ObservableCollection<TeamScore>>((s) =>
                    {
                        LoadVisibilty = Visibility.Visible;
                        Task.Run(() =>
                        {
                            Export.Excel(SourceTeams);
                            LoadVisibilty = Visibility.Hidden;
                        });
                    }
                    );
                }
                return _exportExcelCommand;
            }
        }

        private void ReadExcel()
        {
            var path = "";
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {
                ofd.Multiselect = false;
                ofd.Filter = "Excel 2007|*.xlsx";
                ofd.Title = "请选择数据文件";
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    path = ofd.FileName;
                }
            }
            if (string.IsNullOrEmpty(path))
                return;

            LoadVisibilty = Visibility.Visible;
            Task.Run<DataTable>(() =>
            {
                return Init(path);
            }).ContinueWith(s =>
            {
                DataTable table = s.Result;
                if (table.Rows.Count > 0)
                {
                    StatScore(table);
                }
                LoadVisibilty = Visibility.Hidden;
            });
        }

        private DataTable Init(string filepath)
        {
            var connStr = "";
            connStr = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filepath + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";
            DataTable table = new DataTable();
            try
            {
                OleDbConnection conn = new OleDbConnection(connStr);
                conn.Open();
                DataTable dtSheetName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                var sql = "select * from [Sheet1$]";
                OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
                da.Fill(table);
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取Excel格式错误:" + ex.Message);
            }
            return table;
        }

        private void StatScore(DataTable table)
        {
            try
            {
                GetPersonScore(table);
                StatTeamScore(SourcePlayers);
                ErrorCount = SourceFouls.Count;


                Players = new ObservableCollection<PlayerScore>(SourcePlayers);
                Teams = new ObservableCollection<TeamScore>(SourceTeams);
                FoulPlayers = new ObservableCollection<PlayerScore>(SourceFouls);
            }
            catch (Exception ex)
            {
                MessageBox.Show("统计成交错误:" + ex.Message);
                return;
            }
        }

        private void StatTeamScore(ObservableCollection<PlayerScore> list)
        {
            var groupBIB = from n in list
                           group n.Code.Substring(0, 4) by n.Code.Substring(0, 4) into g
                           select g;

            foreach (var item in groupBIB)
            {
                if (item.Count() == 4)
                {
                    TeamScore data = new TeamScore();
                    var a = list.FirstOrDefault(s => (s.Code == item.Key + "A"));
                    var b = list.FirstOrDefault(s => (s.Code == item.Key + "B"));
                    var c = list.FirstOrDefault(s => (s.Code == item.Key + "C"));
                    var d = list.FirstOrDefault(s => (s.Code == item.Key + "D"));

                    data.Code = item.Key;


                    data.A = a.Name;
                    data.AStart = a.Start;
                    data.AEnd = a.End;
                    var ts = a.End - a.Start;
                    data.AScore = ts;

                    data.B = b.Name;
                    data.BStart = b.Start;
                    data.BEnd = b.End;
                    data.BScore = b.End - b.Start;

                    data.C = c.Name;
                    data.CStart = c.Start;
                    data.CEnd = c.End;
                    data.CScore = c.End - c.Start;

                    data.D = d.Name;
                    data.DStart = d.Start;
                    data.DEnd = d.End;
                    data.DScore = d.End - d.Start;

                    var ab = new TimeSpan();
                    var cd = new TimeSpan();
                    var score = new TimeSpan();
                    if (data.AScore > data.BScore)
                        ab = data.AScore;
                    else
                        ab = data.BScore;
                    if (data.CScore > data.DScore)
                        cd = data.CScore;
                    else
                        cd = data.DScore;

                    if (ab > cd)
                        score = ab;
                    else
                        score = cd;

                    data.GroupScore = score;
                    data.Team = d.Team;

                    Application.Current.Dispatcher.Invoke(() => { SourceTeams.Add(data); });
                }
                else
                {
                    Debug.WriteLine("团队成绩不符合规范:" + item.Key + "=" + item.Count());
                }
            }
            SourceTeams = new ObservableCollection<TeamScore>(SourceTeams.OrderBy(s => s.GroupScore).ToList());
        }

        private TimeSpan GetTimeSpan(string str)
        {
            if (string.IsNullOrEmpty(str))
                return TimeSpan.Zero;

            var start = new DateTime(1970, 1, 1);
            var date = start.AddMilliseconds(Int32.Parse(str));
            var ts = date - start;
            var newTs = new TimeSpan(ts.Hours, ts.Minutes, ts.Seconds);
            return newTs;
        }

        public void GetPersonScore(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                try
                {
                    var data = new PlayerScore
                    {
                        Code = row["BIB"].ToString().Trim(),
                        Name = row["name1"].ToString().Trim(),
                        Team = row["team"].ToString().Trim(),
                        Start = GetTimeSpan(row["start"].ToString().Trim()),
                        Mid = GetTimeSpan(row["mid1"].ToString().Trim()),
                        End = GetTimeSpan(row["finish"].ToString().Trim()),
                    };
                    if (data.Start == TimeSpan.Zero || data.Mid == TimeSpan.Zero || data.End == TimeSpan.Zero)
                    {
                        Debug.WriteLine("没有比赛时间:" + data.Code + " " + data.Name);
                        if (data.Start != TimeSpan.Zero || data.Mid != TimeSpan.Zero || data.End != TimeSpan.Zero)
                        {
                            Application.Current.Dispatcher.Invoke(() => { SourceFouls.Add(data); });
                        }
                        continue;
                    }
                    data.Score = data.End - data.Start;
                    Application.Current.Dispatcher.Invoke(() => { SourcePlayers.Add(data); });
                }
                catch (Exception ex)
                {
                }
            }
        }

        public void Top(int take)
        {
            var query = SourceTeams.Take(take).ToList();
            Teams = new ObservableCollection<TeamScore>(query);
        }

        public void Search(string key)
        {
            if (TabIndex == 0)
            {
                var query = SourcePlayers.Where(s => s.Name.Contains(key) || s.Code.Contains(key));
                Players = new ObservableCollection<PlayerScore>(query);
            }
            else if (TabIndex == 1)
            {
                var query = SourceTeams.Where(s => s.Team.Contains(key) || s.Code.Contains(key));
                Teams = new ObservableCollection<TeamScore>(query);
            }
            else if (TabIndex == 2)
            {
                var query = SourceFouls.Where(s => s.Name.Contains(key) || s.Code.Contains(key));
                FoulPlayers = new ObservableCollection<PlayerScore>(query);
            }
        }
    }
}
