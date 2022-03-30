using System;
using Renci.SshNet;

namespace ssh
{
    class ConexaoSSH
    {
        public ScpClient? Client { get; private set; }
        public string Ip { get; set; }
        public string Serial { get; set; }

        public ConexaoSSH(string ip, string serial)
        {
            Serial = serial;
            Ip = ip;

            if (Serial.Length < 6)
            {
                Serial = "0" + Serial;
            }

            for (int ano = 2016; ano <= 2024; ano++)
            {
                try
                {
                    Client = new ScpClient(Ip, 4022, "root", $"KhompS{Serial}Y{ano}B108R00T00RootPassword");
                    //KhompS105851Y2018B108R00T00RootPassword
                    conectar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                if (Client.IsConnected) break;
            }
        }

        public void conectar()
        {
            Client.Connect();
        }

        public void desconectar()
        {
            Client.Disconnect();
        }

        public void baixarBancoDeDados(string nome)
        {
            try
            {
                using (FileStream fileStream = new FileStream(path: Directory.GetCurrentDirectory() + $"/{nome}", FileMode.Create))
                {
                    Client.Download(filename: "/mnt/kiper/sqlite3/app.db", fileStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        public void enviarBancoDeDados(string nome)
        {
            using (FileStream fs = new FileStream(path: Directory.GetCurrentDirectory() + $"/{nome}", FileMode.Open))
            {
                Client.Upload(fs, "/mnt/kiper/sqlite3/app.db");
            }
        }
    }
}
