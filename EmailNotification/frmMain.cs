using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using Excel;
using PCMSnotification.Properties;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Net.Http;
using System.Net;

namespace PCMSnotification
{
    public delegate void onWriteStatusEvent(string status);
    public delegate void onSetProgressEvent(ProgressInfo value);
    public struct ProgressInfo
    {
        public int ThreadIndex;
        public float Percentage;
        public string CurrentProxy;
        public ProgressInfo(int ThreadIndex, float Percentage,string curproxy)
        {
            this.ThreadIndex = ThreadIndex;
            this.Percentage = Percentage;
            this.CurrentProxy = curproxy;
        }
    }
    public partial class frmMain : Form
    {
        private List<string> _dataSets = new List<string>();
        private List<string> proxylist = new List<string>();
        private List<PcmDataSet> _dataSetsToSave = new List<PcmDataSet>();
        private Thread[] threads;
        private Scrape task = null;
        private int nThreadCount = 1;
        private bool useProxy = true;
        private BackgroundWorker _slave;
        private BackgroundWorker _timemanager;
        private float[] threadprogresses;
        private int[] proxycounter;
        public event onWriteStatusEvent onWriteStatus;
        public event onSetProgressEvent onSetProgress;
        private int totalcount;
        private int finishedcount;
        private float progresspercent;
        private int delay;
        private int elapsedsecond;
        private int remainsecond;
        private DateTime _finishedTime = DateTime.Now;
        private string fileName = string.Empty;
        private string savepath = string.Empty;
        private string saveformat = string.Empty;

        public frmMain()
        {
            InitializeComponent();
            _slave = new BackgroundWorker();
            _slave.WorkerSupportsCancellation = true;
            _slave.DoWork += new DoWorkEventHandler(this.Slave_DoWork);
            this._slave.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.Slave_RunWorkerCompleted);
            saveformat = System.Windows.Forms.Application.StartupPath + "\\{0}_" + DateTime.Now.ToString("yyyyMMdd_HH_mm") + ".csv";
            totalcount = 0;
            finishedcount = 0;
            elapsedsecond = 0;
            remainsecond = 0;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (this.onWriteStatus == null)
                this.onWriteStatus += writeStatus;
            if (this.onSetProgress == null)
                this.onSetProgress += OneCellFinish;

            if (btnStart.Text == "Start")
            {
                //if (txtUserName.Text.Length == 0)
                //{
                //    MessageBox.Show("Please input username");
                //    txtUserName.Focus();
                //    return;
                //}
                //if (txtPassword.Text.Length == 0)
                //{
                //    MessageBox.Show("Please input password");
                //    txtPassword.Focus();F
                //    return;
                //}
                if (txtFileName.Text.Length == 0)
                {
                    MessageBox.Show("Please load excel file");
                    txtFileName.Focus();
                    return;
                }
                if (useProxy && txt_proxypath.Text.Length == 0)
                {
                    MessageBox.Show("Please input Proxy list file");
                    btn_proxy.Focus();
                    return;
                }
                delay = int.Parse(cmbDelay.Text);
                nThreadCount = int.Parse(cmbThread.Text);
                savepath = txtSavePath.Text;
                setSetting();
                saveSetting();

                btnStart.Text = "Pause";

                // Set autoresponder timer
                timerDaily.Enabled = true;
                if (doStart())
                    Constant.bCompleted = false;
                
            }
            else if (btnStart.Text == "Pause")
            {
                btnStart.Text = "Resume";
                timerDaily.Enabled = false;
                doChangeStatus(false);
            }
            else 
            {
                btnStart.Text = "Pause";
                timerDaily.Enabled = false;
                doChangeStatus(true);
            }
        }

        private void copyToSaveList()
        {
            //_dataSetsToSave.Clear();

            //foreach(PcmDataSet data in _dataSets)
            //{
            //    _dataSetsToSave.Add(data);
            //}
        }
        private bool testproxy(string proxyinfo)
        {
            bool ret = false;
            HttpClientHandler handler = new HttpClientHandler()
            {
                Proxy = new WebProxy("http://"+ proxyinfo),
                UseProxy = true,
            };
            HttpClient client = new HttpClient(handler);
            try
            {
                //var byteArray = Encoding.ASCII.GetBytes("username:password1234");
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                HttpResponseMessage response = client.GetAsync("https://google.com").Result;
                HttpContent content = response.Content;

                // ... Check Status Code                                
                if ((int)response.StatusCode == 200)
                    ret = true;
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
            return ret;
        }
        private void loadProxyFile(string proxypath)
        {
            try
            {
                if (!File.Exists(proxypath) || !useProxy)
                    return;
                string[] proxies = File.ReadAllLines(proxypath);
                foreach (string proxyone in proxies)
                {
                    if (proxyone.Split(':').Length == 2 && testproxy(proxyone))
                    {
                        writeStatus(string.Format("Proxy {0} is live", proxyone));
                        proxylist.Add(proxyone);
                    }
                    else
                        writeStatus(string.Format("Proxy {0} is dead", proxyone));
                }
                if (rbtnboth.Checked)
                    proxylist.Add("localhost");
                proxycounter = new int[proxylist.Count];
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
        }
        private void CountProxy(string curproxy)
        {
            try
            {
                for (int i = 0; i < proxylist.Count; i++)
                {
                    if (curproxy == proxylist[i])
                    {
                        proxycounter[i]++;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
        }
        private bool doStart()
        {
            if (_slave.IsBusy)
                return false;

            onWriteStatus("start the job!");

            loadExcelEx(fileName);
            totalcount = _dataSets.Count;
            RefreshProgressBar();
            writeStatus("ProxyChecking now!");
            if (useProxy)
                loadProxyFile(txt_proxypath.Text);
            //copyToSaveList();
            threadprogresses = new float[nThreadCount];
            initStartingVariables();
            int maxcustom = 0;
            //nThreadCount = 2;
            List<string>[] args = new List<string>[nThreadCount];
            for (int i = 0; i < nThreadCount; i++)
            {
                args[i] = new List<string>();
                args[i].Add(i.ToString());
            }
            if (nThreadCount > _dataSets.Count)
                nThreadCount = _dataSets.Count;
            for (int i = 0; i < _dataSets.Count; i++) 
            {
                int mod = i % nThreadCount;
                args[mod].Add(_dataSets[i]);
            }
            threads = new Thread[nThreadCount];
            for (int i = 0; i < nThreadCount; i ++)
            {
                Thread.Sleep(500);
                threads[i] = new Thread(new ParameterizedThreadStart(threadFunc));
                threads[i].Start(args[i]);
            }

            string top_row = "ProductName" + "\t" + "Brand" + "\t" + "Price" + "\t" + "SellerName" + "\t" + "CustomerReviews" + "\t" + "AnsweredQuestions";
            //string custom = string.Empty;
            //while (maxcustom > 0) 
            //{
            //    custom = string.Format("S{0}"+"\t"+"Price{0}"+"\t"+"D{0}"+"\t",maxcustom) + custom;
            //    maxcustom--;
            //}
            //top_row += custom;
            FileWriter writer = new FileWriter(savepath);
            bool result = writer.WriteData(top_row);
            int count = 0;

            //_slave.RunWorkerAsync();

            return true;
        }

        private void RefreshProgressBar()
        {
            try
            {
                string percentstr = progresspercent.ToString("0.0000") + "%";
                int proval = (int)(progresspercent * 10000);
                this.Invoke(new Action(() =>
                {
                    proBar.Maximum = 1000000;
                    proBar.Value = proval;
                    //lbltotal.Text = totalcount.ToString() + "/" + finishedcount.ToString();
                    lblpercent.Text = percentstr;
                }));
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
            
        }
        private void initStartingVariables()
        {
            Constant.bRun = true;
        }

        private void doChangeStatus(bool status)
        {
            //Constant.bRun = false;
            //if(_slave != null)
            //    _slave.CancelAsync();
            if (threads == null)
                return;
            foreach (Thread threadone in threads)
            {
                if (threadone != null)
                {
                    try
                    {
                        if (status)
                            threadone.Resume();
                        else
                            threadone.Suspend();
                    }
                    catch
                    {
 
                    }
                }
            }
        }

        private void btnLoadExcel_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "*.xlsx *.csv Files | *.xlsx;*.csv";
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                txtFileName.Text = fileDlg.FileName;
                fileName = txtFileName.Text;
            }
        }
        private void loadExcelEx(string strFileName)
        {
            try
            {
                _dataSets.Clear();
                string ext = Path.GetExtension(strFileName);
                switch (ext)
                {
                    case ".csv":
                        using (StreamReader sr = new StreamReader(strFileName))
                        {
                            string currentLine = sr.ReadLine();
                            
                            while ((currentLine = sr.ReadLine()) != null) { 
                                if (currentLine != null && currentLine != "") {
                                    currentLine = currentLine.Replace(" ","");
                                    _dataSets.Add(currentLine);
                                }
                            }
                        }
                        break;
                    case ".xlsx":
                        XSSFWorkbook hssfwb;
                        using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
                        {
                            hssfwb = new XSSFWorkbook(file);

                            //Assign the sheet
                            ISheet sheetBuyRate = hssfwb.GetSheet("Buy Rate");
                            ISheet sheetVolume = hssfwb.GetSheet("Volume");
                            if (sheetBuyRate == null || sheetVolume == null)
                                return;

                            for (int i = 0; i <= sheetBuyRate.LastRowNum; i++)
                            {
                                //Get the cell value
                                PcmDataSet data = new PcmDataSet();
                                ICell dateCell = sheetBuyRate.GetRow(i).GetCell(0);
                                //if(dateCell == null)
                                    //data.date = "";
                                //else
                                    //data.date = dateCell.ToString().Trim(); //In the method GetCell you specify the column number you want to read, in the method GetRow you spacify the row

                                //int j = 0;
                                //while(true)
                                //{
                                //    j++;

                                //    ICell cellBuyRate = sheetBuyRate.GetRow(i).GetCell(j);
                                //    if (cellBuyRate == null)
                                //        break;

                                //    string buyRateCol = cellBuyRate.ToString();
                            
                                //    //data.buyRateCols.Add(buyRateCol);

                                //    ICell cellVolume = sheetVolume.GetRow(i).GetCell(j);
                                //    if (cellVolume == null)
                                //        break;

                                //    string volumeCol = cellVolume.ToString();
                            
                                //    //data.volumeCols.Add(volumeCol);
                                //}

                                //_dataSets.Add(data);
                            }

                            file.Close();
                        }
                        break;
                }

                
            }
            catch (Exception e)
            {
                string message = e.Message;
            }
        }

        
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Setting.Instance.captchaKey = "d704341c45ba44bbf7b04c8b21b67700";
            Setting.Instance.captchaKey = "877a3061dc4412b503e7ef10e65336d1";
            this.MaximizeBox = false;
            //this.MinimizeBox = false;
            this.Hide();
            int delay = -1;
            while (++delay <= 60)
                cmbDelay.Items.Add(delay);
            int threads = 0;
            while (++threads <= 24)
                cmbThread.Items.Add(threads);
            cmbThread.Text = "1";
            cmbDelay.Text = "0";
            rbtnname.Checked = true;
            rbtnno.Checked = true;
            txtSavePath.Text = string.Format(saveformat, "amazonname");
            txt_proxyusage.Text = System.Windows.Forms.Application.StartupPath + "\\proxyusage.txt";
            login logfrm = new login();
            DialogResult dlgResult = logfrm.ShowDialog();
            if (dlgResult == DialogResult.OK)
            {
                //MessageBox.Show("LANG = " + login.Lang + "\n");
                this.Show();
            }
            else
            {
                this.Close();
                return;
            }
            loadSetting();
        }

        private string ReadRegistry(string KeyName)
        {
            return Registry.CurrentUser.CreateSubKey("SoftWare").CreateSubKey("DailyScraper").GetValue(KeyName, (object)"").ToString();
        }

        private void WriteRegistry(string KeyName, string KeyValue)
        {
            Registry.CurrentUser.CreateSubKey("SoftWare").CreateSubKey("DailyScraper").SetValue(KeyName, (object)KeyValue);
        }

        private void setSetting()
        {
            //Setting.Instance.username = txtUserName.Text;
            //Setting.Instance.password = txtPassword.Text;
        }

        private void saveSetting()
        {
            try
            {
                //WriteRegistry("username", txtUserName.Text);
                //WriteRegistry("password", txtPassword.Text);
            }
            catch(Exception)
            {

            }
        }

        private void loadSetting()
        {
            try
            {
                //txtUserName.Text = ReadRegistry("username");
                //txtPassword.Text = ReadRegistry("password");
            }
            catch(Exception)
            {

            }
        }

        private void threadFunc(object arg)
        {
            try
            {
                List<string> data = arg as List<string>;
                int ThreadIndex = 0;
                int.TryParse(data[0], out ThreadIndex);
                List<string> param = new List<string>();
                for (int i = 1; i < data.Count; i++)
                    param.Add(data[i]);

                task = new Scrape(ThreadIndex, onWriteStatus, onSetProgress, param, delay, useProxy, proxylist);
                bool result = false;
                if (rbtnname.Checked == true)
                    result = task.doWorkByName(savepath);
                else
                    result = task.doWorkBySeller(savepath);
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
            
            
        }

        private void Slave_DoWork(object sender, DoWorkEventArgs e)
        {
            //task = new Scrape(onWriteStatus, onSetProgress, _dataSets,delay);
            //PcmDataSet todayData = task.doWork(savepath);
            //if (todayData == null)
            //    return;

            //_dataSetsToSave.Add(todayData);

            //writeExcel();
        }

        private void Slave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Constant.bCompleted = true;
            
            if (task != null)
            {
                task.doRelease();
            }

            _finishedTime = DateTime.Now;
        }

        private void writeExcel()
        {
            try
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);

                using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    //int nRow = -1;
                    //IWorkbook wb = new XSSFWorkbook();
                    //ISheet sheetBuyRate = wb.CreateSheet("Buy Rate");
                    //ISheet sheetVolume = wb.CreateSheet("Volume");

                    //foreach (PcmDataSet data in _dataSetsToSave)
                    //{
                    //    nRow++;

                    //    IRow rowBuyRate = sheetBuyRate.CreateRow(nRow);
                    //    IRow rowVolume = sheetVolume.CreateRow(nRow);

                    //    rowBuyRate.CreateCell(0).SetCellValue(data.date);
                    //    rowVolume.CreateCell(0).SetCellValue(data.date);

                    //    for (int j = 0; j < data.buyRateCols.Count; j++)
                    //    {
                    //        ICell buyRateCell = rowBuyRate.CreateCell(j + 1);
                    //        buyRateCell.SetCellValue(data.buyRateCols[j]);
                    //        if(nRow != 0)
                    //        {
                    //            ICellStyle testeStyle = wb.CreateCellStyle();
                    //            testeStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Medium;
                    //            testeStyle.FillForegroundColor = getCellColor(data.buyRateCols[j]);
                    //            testeStyle.FillPattern = FillPattern.SolidForeground;

                    //            buyRateCell.CellStyle = testeStyle;
                    //        }
                    //    }

                    //    for(int j = 0; j < data.volumeCols.Count; j ++)
                    //    {
                    //        ICell volumeCell = rowVolume.CreateCell(j + 1);
                    //        volumeCell.SetCellValue(data.volumeCols[j]);
                    //    }
                    //}

                    //wb.Write(stream);
                    //stream.Close();
                }
            }
            catch (Exception e)
            {

            }
        }

        private short getCellColor(string value)
        {
            double val = 0;
            if(double.TryParse(value.Replace("%", "").Trim(), out val))
            {
                if (val > 50)
                    return IndexedColors.Green.Index;
                else
                    return IndexedColors.Red.Index;
            }

            return IndexedColors.White.Index;
        }

        private void writeStatus(string status)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    lblStatus.Text = status;
                }));
            }
            catch(Exception)
            {

            }
        }
        private void OneCellFinish(ProgressInfo proinfo)
        {
            int index = proinfo.ThreadIndex;
            threadprogresses[index] = proinfo.Percentage;
            if (proinfo.CurrentProxy != "")
                CountProxy(proinfo.CurrentProxy);
            float totalprogress = 0;
            foreach (float proone in threadprogresses)
                totalprogress += proone;
            progresspercent = totalprogress / (float)nThreadCount;
            if (progresspercent >= 100)
                progresspercent = 100;
            int totalsec = 0;
            if (progresspercent > 0)
                totalsec = (int)(elapsedsecond * 100 / progresspercent);
            remainsecond = totalsec - elapsedsecond;
            //finishedcount++;
            RefreshProgressBar();
        }

        private void timerDelay_Tick(object sender, EventArgs e)
        {
            elapsedsecond++;
            remainsecond--;
            RefreshTime();
            if (progresspercent >= 100) 
            {
                totalcount = finishedcount;
                timerDaily.Enabled = false;
                KillAllProcess();
                if (useProxy && txt_proxyusage.Text != "")
                    SaveProxyUsage(txt_proxyusage.Text);
                if (MessageBox.Show("All progresses are successfully finished!", "Finished!", MessageBoxButtons.OK) == DialogResult.OK)
                    Application.Exit();
            }
            
        }
        private void SaveProxyUsage(string path)
        {
            try
            {
                StreamWriter sw = new StreamWriter(path, false, Encoding.Unicode);
                
                for (int i = 0; i < proxylist.Count; i++)
                    sw.WriteLine(string.Format("Proxy {0} scraped {1} items",proxylist[i],proxycounter[i]));
                sw.Close();
            }
            catch (Exception e)
            {
                string error = e.Message;

            }
        }
        private string convertSeconds(int secs)
        {
            if (secs < 0)
                return "--h:--m:--s";
            int nhours = secs / 3600;
            TimeSpan t = TimeSpan.FromSeconds(secs);
            if (nhours < 100)
                return string.Format("{0:D2}h:{1:D2}m:{2:D2}s", nhours, t.Minutes, t.Seconds);
            else
                return string.Format("{0:D3}h:{1:D2}m:{2:D2}s", nhours, t.Minutes, t.Seconds);
        }
        private void RefreshTime()
        {
            try
            {
                string timestr = convertSeconds(elapsedsecond) + "/" + convertSeconds(remainsecond);
                this.Invoke(new Action(() =>
                {
                    lbltime.Text = timestr;
                }));
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
        }
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveSetting();
            KillAllProcess();
        }
        private void KillAllProcess()
        {
            if (threads == null)
                return;
            try
            {
                foreach (Thread threadone in threads)
                {
                    if (threadone != null)
                        threadone.Abort();
                }
                Process[] ps = Process.GetProcessesByName("chromedriver");
                foreach (Process p in ps)
                    p.Kill();
                ps = Process.GetProcessesByName("chrome");
                foreach (Process p in ps)
                    p.Kill();
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
        }
        private void btnSavePath_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "*.csv Files | *.csv";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                txtSavePath.Text = sfd.FileName;
                savepath = txtSavePath.Text;
            }
        }

        private void rbtnname_CheckedChanged(object sender, EventArgs e)
        {
            txtSavePath.Text = string.Format(saveformat, "amazonname");
        }

        private void rbtnseller_CheckedChanged(object sender, EventArgs e)
        {
            txtSavePath.Text = string.Format(saveformat, "amazonseller");
        }

        private void rbtnno_CheckedChanged(object sender, EventArgs e)
        {
            txt_proxypath.Enabled = false;
            btn_proxy.Enabled = false;
            txt_proxyusage.Enabled = false;
            btn_proxyusage.Enabled = false;
            useProxy = false;
        }

        private void rbtnproxy_CheckedChanged(object sender, EventArgs e)
        {
            txt_proxypath.Enabled = true;
            btn_proxy.Enabled = true;
            txt_proxyusage.Enabled = true;
            btn_proxyusage.Enabled = true;
            useProxy = true;
        }

        private void rbtnboth_CheckedChanged(object sender, EventArgs e)
        {
            txt_proxypath.Enabled = true;
            btn_proxy.Enabled = true;
            txt_proxyusage.Enabled = true;
            btn_proxyusage.Enabled = true;
            useProxy = true;
        }

        private void btn_proxy_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "*.txt Files | *.txt";
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                txt_proxypath.Text = fileDlg.FileName;
            }
        }

        private void btn_proxyusage_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "*.txt Files | *.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                txt_proxyusage.Text = sfd.FileName;
            }
        }
    }
}
