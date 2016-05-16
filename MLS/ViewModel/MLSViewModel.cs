using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.NotifyBase;

namespace MLS.ViewModel
{
    public class MLSViewModel : PropertyNotifyObject
    {
        public ObservableCollection<PersonScore> SourceList { get; set; }
        public ObservableCollection<PersonScore> Errors { get; set; }
        public ObservableCollection<TeamScore> Team { get; set; }

        public MLSViewModel()
        {
            SourceList = new ObservableCollection<PersonScore>();
            Team = new ObservableCollection<TeamScore>();
            Errors = new ObservableCollection<PersonScore>();
        }

        public int ErrorCount
        {
            get { return this.GetValue(s => s.ErrorCount); }
            set { this.SetValue(s => s.ErrorCount, value); }
        }

        public void Init()
        {
            var connStr = "";
            var fileName = @"D:\Project\mls\mls\bin\Debug\ysj.xlsx";
            connStr = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + fileName + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";
            try
            {
                OleDbConnection conn = new OleDbConnection(connStr);
                conn.Open();
                DataTable dtSheetName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                var sql = "select * from [Sheet1$]";
                OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
                DataTable table = new DataTable();
                da.Fill(table);

                SourceList = ToPersonScore(table);
                Team = StatTeamScore(SourceList);
                ErrorCount = Errors.Count;
            }
            catch (Exception ex)
            {
            }
        }

        private ObservableCollection<TeamScore> StatTeamScore(ObservableCollection<PersonScore> list)
        {
            var groupBIB = from n in list
                           group n.Code.Substring(0, 4) by n.Code.Substring(0, 4) into g
                           select g;

            ObservableCollection<TeamScore> statList = new ObservableCollection<TeamScore>();
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
                    statList.Add(data);
                }
                else
                {
                    Debug.WriteLine("团队成绩不符合规范:" + item.Key + "=" + item.Count());
                }
            }
            //statList = statList.OrderBy(s => s.GroupScore).ToList();
            return statList;
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

        public ObservableCollection<PersonScore> ToPersonScore(DataTable table)
        {
            ObservableCollection<PersonScore> list = new ObservableCollection<PersonScore>();
            foreach (DataRow row in table.Rows)
            {
                try
                {
                    var data = new PersonScore
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
                            Errors.Add(data);
                        continue;
                    }
                    data.Score = data.End - data.Start;
                    list.Add(data);
                }
                catch (Exception ex)
                {
                }
            }
            return list;
        }
    }
}
