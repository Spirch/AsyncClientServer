using AsyncClientServer;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TestClientServer
{
	internal delegate void AppendTextDelegate(string msg);
	internal delegate void HandleTitleChange(string msg);
	
	public partial class Form1 : Form
	{

		private const string AddressTest = "127.0.0.1";
		private const int PortTest = 54321;

		private Client client;
		private Server server;
		private Random rnd;

		private RandomNumberGenerator secureRnd;

		private bool isClient;

		public Form1(bool client, bool enableTimer, bool randomData, bool disconnect, bool append)
		{
			InitializeComponent();
			this.Text = client ? "client" : "Server";
			rnd = new Random();
			secureRnd = RandomNumberGenerator.Create();

			isClient = client;
			chkTimer.Checked = enableTimer;
			chkData.Checked = randomData;
			chkDisconnect.Checked = disconnect;
			chkAppend.Checked = append;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			textBox2.KeyDown += new KeyEventHandler(textBox2_KeyDown);

			if (chkTimer.Checked && !isClient)
			{
				HandleServer();
			}

			timer1.Interval = rnd.Next(500, 2000);
			timer1.Enabled = chkTimer.Checked;
		}

		void textBox2_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				Send(textBox2.Text);
				textBox2.Text = "";
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			if (client != null)
			{
				this.Text = "Client - Closed";
				client.Close();
			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			if (server != null)
			{
				timer1.Enabled = false;
				this.Text = "Server - Closed";
				server.StopServer();
			}
		}

		private void Send(string msg)
		{
			if (!string.IsNullOrWhiteSpace(msg))
			{ 
				if (client != null)
				{
					client.Send(msg);
				}
				else if (server != null)
				{
					HandleNewMsg(Environment.NewLine + msg);
					server.SendAll(msg);
				}
			}
		}

		private void HandleTitleChange(string msg)
		{
			if (this.InvokeRequired)
			{
				this.BeginInvoke(new HandleTitleChange(HandleTitleChange), new object[] { msg });
			}
			else
			{
				this.Text = msg;
			}
		}

		private void HandleNewMsg(string msg)
		{			
			if(chkAppend.Checked)
			{
				if (this.InvokeRequired)
				{
					this.BeginInvoke(new AppendTextDelegate(HandleNewMsg), new object[] { msg });
				}
				else
				{
					textBox1.AppendText(msg);
				}
			}
		}

		#region "Client"

		private void button2_Click(object sender, EventArgs e)
		{
			HandleClient();
		}

		private void HandleClient()
		{
			if (client == null)
			{
				client = new Client();
				client.MessageReceived += client_MessageReceived;
				client.Disconnected += client_ConnectionClosed;
				client.Connected += client_Connected;
				client.SocketError += client_SocketError;
				client.ReceivedClientId += client_ReceivedClientId;
				client.ReceivedListClientId += client_ReceivedListClientId;
			}

			client.Connect(AddressTest, PortTest);
		}

		void client_ReceivedListClientId(Client client, IEnumerable<int> ids)
		{
			this.Invoke(new Action(() =>
			{
				listBox1.Items.Clear();
				listBox1.Items.AddRange(ids.Select(x => (object)x).ToArray());
			}));			
		}

		void client_ReceivedClientId(Client client)
		{
			HandleTitleChange("Client - Connected - " + client.Id);
		}

		private void client_SocketError(Client client, Exception e)
		{
			MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace);
		}

		void client_Connected(Client client)
		{
			HandleTitleChange("Client - Connected - Unknown Id");
			HandleNewMsg(Environment.NewLine + "Connected!");
		}

		private void client_ConnectionClosed(Client client)
		{
			timer1.Enabled = false;
			HandleTitleChange("Client - Disconnected");
			HandleNewMsg(Environment.NewLine + "Disconnected!");
		}

		private void client_MessageReceived(Client client, byte[] msg, KindMessage kindOfSend)
		{
			if (msg != null)
			{
				HandleNewMsg(Environment.NewLine + Encoding.UTF8.GetString(msg));
			}
		}
		#endregion

		#region "Server"

		private void button1_Click(object sender, EventArgs e)
		{
			HandleServer();
		}

		private void HandleServer()
		{
			if (server == null)
			{
				server = new Server();
				server.MessageReceived += server_MessageReceived;
				server.Disconnected += server_ConnectionClosed;
				server.Connected += server_Connected;
				server.SocketError += server_SocketError;
			}
			this.Text = "Server - listening";
			server.StartServer(AddressTest, PortTest);
		}

		private void server_SocketError(Client client, Exception e)
		{
			MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace);
		}

		void server_Connected(int id)
		{
			HandleNewMsg(Environment.NewLine + id.ToString() + ": Connected!");
			server.Send(id, "Hello ClientId " + id.ToString());
		}

		private void server_ConnectionClosed(int id)
		{
			HandleNewMsg(Environment.NewLine + id.ToString() + ": Disconnected!");
		}

		private void server_MessageReceived(int id, byte[] msg, KindMessage kindOfSend)
		{
			if (msg != null)
			{
				var text = Encoding.UTF8.GetString(msg);
				text = string.Format("From Client Id {0}: {1}", id, text);
				HandleNewMsg(Environment.NewLine + text);
				if (kindOfSend == KindMessage.Message)
				{
					server.SendAll(text);
				}
			}
		}
		#endregion

		private void button5_Click(object sender, EventArgs e)
		{
			if (client != null)
				Task.Factory.StartNew(() => {
					for (int i = 0; i < 5; ++i)
					{
						string val = "";

						for (int j = 0; j < 42; ++j)
						{
							val = string.Format("{0}{1}",  val, j) ;
							client.Send(i.ToString("000") + " " + val);
						}
					}
				},TaskCreationOptions.LongRunning);
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			timer1.Interval = rnd.Next(400, 1500);

			if (chkDisconnect.Checked && client != null && client.IsConnected())
			{
				var reconnect = new byte[1];

				secureRnd.GetBytes(reconnect);

				if (reconnect[0] <= 10)
				{
					client.Close();
					timer1.Enabled = true;
					return;
				}
			}

			if (server == null && client == null || (client != null && !client.IsConnected()))
			{
				HandleClient();
			}

			if(chkData.Checked)
			{
				var msg = new byte[rnd.Next(25, 80)];

				secureRnd.GetBytes(msg);

				Send(DateTime.Now.ToLongTimeString() + " : " + (server != null ? "server" : "client") + " : " + Convert.ToBase64String(msg));
			}


			//timer1.Enabled = false;
		}

		private void button6_Click(object sender, EventArgs e)
		{
			if(client != null)
			{
				MessageBox.Show(client.Id.ToString());
			}
		}

		private void chkTimer_CheckedChanged(object sender, EventArgs e)
		{
			timer1.Enabled = chkTimer.Checked;
		}

		private void button7_Click(object sender, EventArgs e)
		{
			if (client != null)
			{
				client.RequestListOfConnectedCliendId();
			}
		}
	}
}
