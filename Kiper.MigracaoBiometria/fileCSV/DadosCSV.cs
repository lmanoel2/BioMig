using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Kiper.MigracaoBiometria
{
    public class DadosCSV : ReadFile
    {

        public int[] PosicaoDeConversao { get; private set; }
        public string TextoCSV { get; set; }

        public DadosCSV() : base()
        {
        }

        public DadosCSV(string caminhoOriginal, string caminhoEditado, string[] chave) : base(caminhoOriginal, caminhoEditado, chave)
        {
        }

        public void SalvarDados()
        {
            string linha = "";
            string[] linhaseparada = null;

            try
            {
                Reader = new StreamReader(CaminhoOriginal, Encoding.UTF8);
            }
            catch
            {
                Console.WriteLine($"Não foi possível abrir o arquivo no caminho: {CaminhoOriginal}");
                Console.WriteLine("Pressione qualquer tecla para sair...");
                Console.ReadKey();
                Environment.Exit(0);
            }


            PosicaoDeConversao = new int[Chave.Length];

            for (int i = 0; i < PosicaoDeConversao.Length; i++)
            {
                PosicaoDeConversao[i] = -1;
            }

            for (int i = 0; i < QuantidadeLinhas; i++)
            {
                linha = Reader.ReadLine();
                linhaseparada = linha.Split(',');


                for (int j = 0; j < QuantidadeColunas; j++)
                {

                    Matriz[i, j] = linhaseparada[j];

                    if (i == 0)
                    {
                        string titulo = Regex.Replace(linhaseparada[j], "[^0-9a-zA-Z]+", "");
                        int cont = 0;
                        foreach (string chave in Chave)
                        {
                            if (chave == titulo) PosicaoDeConversao[cont] = j;
                            cont++;
                        }
                    }

                }
            }
            Reader.Close();
            foreach (int valor in PosicaoDeConversao)
            {
                if (valor == -1)
                {
                    Console.WriteLine($"Erro ao encontrar os parametros principais");
                    Console.WriteLine("Pressione qualquer tecla para sair...");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
        }

        public void EscreverDados()
        {
            File.WriteAllText(CaminhoEditado, TextoCSV);
        }

        public void FormatarDados()
        {
            string strSeperator = ",";
            StringBuilder sbOutput = new StringBuilder();

            for (int i = 0; i < QuantidadeLinhas; i++)
            {
                for (int j = 0; j < QuantidadeColunas; j++)
                {
                    sbOutput.Append(string.Join(strSeperator, Matriz[i, j]));
                    sbOutput.Append(",");
                }
                sbOutput.AppendLine("");
            }
            TextoCSV = sbOutput.ToString();
        }
    }
}
