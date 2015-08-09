using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace TestClientServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new Form2();
            config.ShowDialog();

            var s1 = new Form1(false, config.startServer(), config.serverRandomData(), false, config.serverAppend());
            s1.Show();
            for (int i = 0; i < config.numberofclient(); ++i)
            {
                new Form1(true, config.startClient(), config.clientRandomData(), config.clientDisconnect(),config.clientAppend()).Show();
            }
            s1.BringToFront();

            config.Dispose();
            config = null;

            Application.Run();
        }
    }
}
