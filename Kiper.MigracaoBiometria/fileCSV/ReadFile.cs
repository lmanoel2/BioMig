using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Kiper.MigracaoBiometria
{
    public class ReadFile
    {
        public string CaminhoOriginal { get; set; }
        public string CaminhoEditado { get; set; }
        public int QuantidadeLinhas { get; set; }
        public int QuantidadeColunas { get; set; }
        public string[] Chave { get; set; }
        public string[,] Matriz { get; private set; }

        public StreamReader Reader { get; set; }
        public ReadFile()
        {

        }
        public ReadFile(string caminhoOriginal, string caminhoEditado, string[] chave)
        {
            CaminhoOriginal = caminhoOriginal;
            CaminhoEditado = caminhoEditado;
            Chave = chave;
            ContaLinhasColunas();
            Matriz = new string[QuantidadeLinhas, QuantidadeColunas];
        }

        public void ContaLinhasColunas()
        {
            string linha = "";
            string[] linhaseparada = null;
            int contLinhas = 0;
            int contColunas = 0;

            try
            {
                Reader = new StreamReader(CaminhoOriginal, Encoding.UTF8);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error - {ex.Message}");
                //Console.WriteLine($"Não foi possível abrir o arquivo no caminho: {CaminhoOriginal}\n\n");
                Console.WriteLine("\nCertifique-se que:");
                Console.WriteLine("1- A planilha não está aberta no computador");
                Console.WriteLine("2- O nome do arquivo está correto (biometria.csv)");
                Console.WriteLine("3- O arquivo se encontra no mesmo local do executavel");
                Console.WriteLine("Pressione qualquer tecla para sair...");
                Console.ReadKey();
                Environment.Exit(0);
            }

            bool primeiraVez = true;
            while (true)
            {
                linha = Reader.ReadLine();
                if (linha == null) break;

                linhaseparada = linha.Split(',');
                contLinhas += 1;
                if (primeiraVez) contColunas = linhaseparada.Length;
                primeiraVez = false;
            }
            QuantidadeLinhas = contLinhas;
            QuantidadeColunas = contColunas;
            Reader.Close();
        }

    }
}
