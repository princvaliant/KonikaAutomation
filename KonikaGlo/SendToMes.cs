using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KonikaGlo
{
    public partial class SendToMes : Form
    {
        private static string URL = "http://mes/glo/dataImport/konica";

        public static decimal _red = 0;
        public static decimal _blue = 0;
        public static decimal _green = 0;
        public String screenSize = "";

        public SendToMes()
        {
            InitializeComponent();

            red.Value = _red;
            green.Value = _green;
            blue.Value = _blue;
        }

        private void serialNumber_Leave(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainForm mainForm = (MainForm)Application.OpenForms[0];

            btnSend.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;

            Dictionary<String, Object> json = new Dictionary<String, Object>();
            json.Add("data", mainForm.results);
            json.Add("summary", mainForm.summaries);
            json.Add("raw", mainForm.rawData);
            json.Add("code", serialNumber.Text);
            json.Add("comment", comment.Text);
            json.Add("testedBy", mainForm.user.Text);
            json.Add("red_current", red.Value);
            json.Add("green_current", green.Value);
            json.Add("blue_current", blue.Value);
            json.Add("temperature", temperature.Value);
            json.Add("exposure", mainForm.exposure.Text);
            json.Add("screenSize", screenSize);

            _red = red.Value;
            _green = green.Value;
            _blue = blue.Value;

            String response = getHttpResponse(JsonConvert.SerializeObject(json));

            if (response.IndexOf("Exception") >= 0 || response.IndexOf("false") >= 0)
            {
                btnSend.Enabled = true;
                Cursor.Current = Cursors.Default;
                MessageBox.Show(response, "Error occured");
            } else {
                btnSend.Enabled = true;
                Cursor.Current = Cursors.Default;
                MessageBox.Show(response, "Success");
                mainForm.sendToMes.Enabled = false;
                this.Close();
            }
        }

        public static string getHttpResponse(string data)
        {
            string responseData = String.Empty;
            HttpWebRequest req = null;
            HttpWebResponse resp = null;
            StreamReader strmReader = null;
            try
            {
                req = (HttpWebRequest)HttpWebRequest.Create(URL);// set HttpWebRequest properties here (Method, ContentType, etc), some code in case of POST you need to post data
                req.Method = "POST";
                req.Headers.Add("Accept-Language", "en-us\r\n");
                req.Headers.Add("UA-CPU", "x86 \r\n");
                req.Headers.Add("Cache-Control", "no-cache\r\n");
                req.ContentType = "application/json; charset=utf-8";
                if ((data != null) && (data.Length > 0))
                {
                    StreamWriter writer = new StreamWriter(req.GetRequestStream());
                    writer.Write(data);
                    writer.Close();
                }
                resp = (HttpWebResponse)req.GetResponse();
                strmReader = new StreamReader(resp.GetResponseStream());
                responseData = strmReader.ReadToEnd().Trim();
            }
            catch (Exception ex)
            {
                responseData = "Exception connecting to MES: " + URL + ", "  + ex.Message + "\n\r" + ex.StackTrace;
            }
            finally
            {
                if (req != null)
                {
                    req = null;
                }
                if (resp != null)
                {
                    resp.Close();
                    resp = null;
                }
            }
            return responseData;
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
