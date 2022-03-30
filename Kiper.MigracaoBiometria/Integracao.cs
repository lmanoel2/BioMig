using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiper.MigracaoBiometria
{
    public class Integracao
    {
        public DadosCSV DadosBiometria { get; set; }
        public DadosCSV DadosIntegracao { get; set; }
        public string[] IdMonitoring { get; set; }
        public Dictionary<long, long>? DictIDs { get; set; }
        public List<long> ListUserSuccess { get; set; }
        public List<long> ListUserFailed { get; set; }
        
        public Integracao(DadosCSV dadosBiometria, Dictionary<long, long>? dictIDs)
        {
            DadosBiometria = dadosBiometria;
            DictIDs = dictIDs;
            ListUserSuccess = new List<long>();
            ListUserFailed = new List<long>();
        }

        public void ConverterIDs()
        {
            int cont = 0;

            foreach (string id in IdMonitoring)
            {
                foreach (int pos in DadosBiometria.PosicaoDeConversao)
                {
                    DadosBiometria.Matriz[cont, pos] = id;
                }
                cont++;
            }
        }

        public void IdentificarUserID()
        {
            for (int i = 1; i < DadosBiometria.QuantidadeLinhas; i++)
            {
                IdMonitoring[i] = DadosBiometria.Matriz[i, DadosBiometria.PosicaoDeConversao[0]];
                for (int j = 1; j < DadosIntegracao.QuantidadeLinhas; j++)
                {
                    if (DadosBiometria.Matriz[i, DadosBiometria.PosicaoDeConversao[0]] == DadosIntegracao.Matriz[j, DadosIntegracao.PosicaoDeConversao[0]])
                    {
                        IdMonitoring[i] = DadosIntegracao.Matriz[j, DadosIntegracao.PosicaoDeConversao[1]];
                    }
                }
            }
        }

        public void ConverterIDsNew()
        {
            bool sincronizado = false;

            for (int i = 1; i < DadosBiometria.QuantidadeLinhas; i++)
            {
                sincronizado = false;
                foreach (KeyValuePair<long, long> id in DictIDs)
                {
                    if (id.Key == int.Parse(DadosBiometria.Matriz[i, DadosBiometria.PosicaoDeConversao[0]]))
                    {
                        //ID.KEY == SIGMA
                        //ID.VALUE == MONITORING

                        ListUserSuccess.Add(int.Parse(id.Value.ToString()));

                        foreach (int pos in DadosBiometria.PosicaoDeConversao)
                        {
                            DadosBiometria.Matriz[i, pos] = id.Value.ToString();
                        }
                        sincronizado = true;
                    }
                }
                if (!sincronizado)
                {
                    ListUserFailed.Add(int.Parse(DadosBiometria.Matriz[i, DadosBiometria.PosicaoDeConversao[0]]));
                }
            }            
        }

    }
}

