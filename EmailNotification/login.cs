using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace PCMSnotification
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
        }
        private int ctnpasswrong;

        private void btnEnter_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Host = "example.com";
            Task<string> task = client.GetStringAsync("http://192.64.86.68/authentication?userid="+txtUser.Text+"&password="+txtPasswd.Text);
            task.Wait();
            string result = task.Result;
            switch (result)
            {
                case "user":
                    if (MessageBox.Show("This username is invalid.\rPlease ensure the username!", "Username invalid!", MessageBoxButtons.OK) == DialogResult.OK && ++ctnpasswrong == 3)
                        Application.Exit();
                    break;
                case "passwd":
                    if (MessageBox.Show("Password is incorrect.\rPlease ensure the password!", "Password incorrect!", MessageBoxButtons.OK) == DialogResult.OK && ++ctnpasswrong == 3)
                        Application.Exit();
                    break;
                case "expired":
                    if (MessageBox.Show("This user is already expired.\rPlease choose another user!", "User expired!", MessageBoxButtons.OK) == DialogResult.OK && ++ctnpasswrong == 3)
                        Application.Exit();
                    break;
                case "OK":
                    this.DialogResult = DialogResult.OK;
                    break;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
        
        
        private void login_Load(object sender, EventArgs e)
        {
            ctnpasswrong = 0;
            
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            
            this.AcceptButton = btnEnter;
        }
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        private void login_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
