using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestClientServer
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public bool startServer()
        {
            return chkStartServer.Checked;
        }

        public bool serverRandomData()
        {
            return chkServerSendData.Checked;
        }

		public bool serverAppend()
		{
			return chkServerAppend.Checked;
		}


        public bool startClient()
        {
            return chkStartClient.Checked;
        }
        public bool clientRandomData()
        {
            return chkClientSendData.Checked;
        }

        public bool clientDisconnect()
        {
            return chkClientDisconnect.Checked;
        }

		public bool clientAppend()
		{
			return chkClientAppend.Checked;
		}

        public int numberofclient()
        {
            return (int)nudClient.Value;
        }

		private void Form2_Load(object sender, EventArgs e)
		{

		}
    }
}
