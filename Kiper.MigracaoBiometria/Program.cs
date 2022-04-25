using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ssh;
using banco_de_dados;
using Kiper.MigracaoBiometria.Autenticacao;

namespace Kiper.MigracaoBiometria
{
    class Program
    {

        static async Task Main(string[] args)
        {
            string texto = @" __  ___  __  .______    _______ .______         .______    __    ______   .___  ___.  __    _______ 
|  |/  / |  | |   _  \  |   ____||   _  \        |   _  \  |  |  /  __  \  |   \/   | |  |  /  _____|
|  '  /  |  | |  |_)  | |  |__   |  |_)  |       |  |_)  | |  | |  |  |  | |  \  /  | |  | |  |  __  
|    <   |  | |   ___/  |   __|  |      /        |   _  <  |  | |  |  |  | |  |\/|  | |  | |  | |_ | 
|  .  \  |  | |  |      |  |____ |  |\  \----.   |  |_)  | |  | |  `--'  | |  |  |  | |  | |  |__| | 
|__|\__\ |__| | _|      |_______|| _| `._____|   |______/  |__|  \______/  |__|  |__| |__|  \______|";


            Login login = new Login();
            Console.WriteLine(texto);
            Console.WriteLine("-----------------------------------");

            Console.Write("Usuario Monitoring: ");
            string? username = Console.ReadLine();

            Console.Write("Senha: ");            
            string? password = login.LerSenha();
            Console.WriteLine("-----------------------------------\n\n");

            Console.WriteLine("Autenticando Usuário...");


            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://migration-api-cloud.kiper.com.br")
            };


            var response = await httpClient.PostAsync("/Auth/SignIn", new StringContent(JsonSerializer.Serialize(new
            {
                username,
                password
            }), encoding: System.Text.Encoding.UTF8, mediaType: "text/json"));

            Console.Clear();

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.Clear();
                Console.WriteLine("Usuário incorreto!\n\n");

                Console.WriteLine("Digite qualquer tecla para sair...");
                Console.ReadKey();
                Environment.Exit(0);
            }

            Console.WriteLine(texto);
            Console.WriteLine("Usuário Autenticado!\n\n");

            var content = await response.Content.ReadAsStreamAsync();

            JsonElement contentDeserialized = await JsonSerializer.DeserializeAsync<JsonElement>(content);

            Console.WriteLine("-----------------------------------");
            Console.WriteLine("++ INFORMAÇÕES DO PARCEIRO SIGMA ++");
            Console.Write("ID Condominio Sigma: ");
            int idSigma = int.Parse(Console.ReadLine());
            Console.Write("ID Parceiro Sigma: ");
            int idParceiro = int.Parse(Console.ReadLine());
            Console.WriteLine("-----------------------------------\n\n");

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", contentDeserialized.GetProperty("accessToken").ToString());


            var response2 = await httpClient.GetAsync($"/migration/GetDwellersMapping/{idParceiro}/{idSigma}");
            var content2 = await response2.Content.ReadAsStreamAsync();
            List<MapeamentoIDs>? responseContentDeserialized = await JsonSerializer.DeserializeAsync<List<MapeamentoIDs>>(content2);

            Dictionary<long, long>? idsDictionary = responseContentDeserialized?.ToDictionary(x => x.IdSigma, elementSelector: x => x.IdMonitoring);

            //Dictionary<long, long>? idsDictionary = new Dictionary<long, long>();
            //idsDictionary.Add(533, 1);
            //idsDictionary.Add(1071, 2);
            //idsDictionary.Add(2941, 3);
            

            //----------------------------------------------------------------------------------------------------------------------------------------

            Console.WriteLine("Iniciando integração!\n");

            string nomeArquivoBiometria = "biometria.csv";
            string nomeArquivoFinal = "Tabela_FINAL.csv";
            string nomeUserSuccess = "usuariosSuccess.csv";
            string nomeUserFailed = "usuariosFailed.csv";

            string caminhoBiometria = Path.GetFullPath(nomeArquivoBiometria);
            string caminhoArquivoFinal = Path.GetFullPath(nomeArquivoFinal);
            string caminhoUserSuccess = Path.GetFullPath(nomeUserSuccess);
            string caminhoUserFailed = Path.GetFullPath(nomeUserFailed);

            string dbOriginal = "app.db";
            string dbEditado = "appEdit.db";
            string dbEditado2 = "appExcluir.db";

            string[] chave1 = { "ID", "Nome", "IDnico" };

            Console.WriteLine("---------------------------------------------------------------------");
            Console.Write("Insira o IP da CPU: ");
            string? ipCPU = Console.ReadLine();
            Console.Write("\nInsira o Serial da CPU: ");
            string? serialCPU = Console.ReadLine();
            Console.WriteLine("---------------------------------------------------------------------\n");
            

            ConexaoSSH client = new (ipCPU, serialCPU);
            client.baixarBancoDeDados(dbOriginal);
            client.baixarBancoDeDados(dbEditado);


            DadosCSV dadosBiometria = new DadosCSV(caminhoBiometria, caminhoArquivoFinal, chave1);

            Console.WriteLine("Salvando dados...\n");
            dadosBiometria.SalvarDados();
            Console.WriteLine("Dados salvos!\n");

            Integracao dadosAPI = new Integracao(dadosBiometria, idsDictionary);
            dadosAPI.ConverterIDsNew();

            Console.WriteLine("Formatando os dados...\n");
            dadosBiometria.FormatarDados();

            Console.Clear();
            Console.WriteLine(texto);

            Console.WriteLine($"\nRegistrando BIOMETRIAS no arquivo CSV ({nomeArquivoFinal})\n");
            dadosBiometria.EscreverDados();


            //----------------------------------------------------------------------------------------------------------------------------------------
            string strSeperator = ",";
            StringBuilder sbUserSuccess = new StringBuilder();
            StringBuilder sbUserFailed = new StringBuilder();

            sbUserSuccess.Append(string.Join(strSeperator, dadosAPI.ListUserSuccess));
            sbUserSuccess.Append(",");

            string TextoCSV = sbUserSuccess.ToString();
            File.WriteAllText(caminhoUserSuccess, TextoCSV);

            sbUserFailed.Append(string.Join(strSeperator, dadosAPI.ListUserFailed));
            sbUserFailed.Append(",");

            TextoCSV = sbUserFailed.ToString();
            File.WriteAllText(caminhoUserFailed, TextoCSV);


            Console.WriteLine("---------------------------------------------------------------------");
            Console.WriteLine($"Quantidade de Usuários Sincronizados: {dadosAPI.ListUserSuccess.Count}");
            Console.WriteLine($"Quantidade de Usuários Não Sincronizados: {dadosAPI.ListUserFailed.Count}");
            Console.WriteLine("---------------------------------------------------------------------\n");


            Console.WriteLine($"Lista de usuarios sincronizados [{nomeUserSuccess}] salvo em: {caminhoUserSuccess}\n\n");
            Console.WriteLine($"Lista de usuários não sincronizados [{nomeUserFailed}] salvo em: {caminhoUserFailed}\n\n");


            Console.WriteLine("Deseja atualizar o banco de dados da CPU? (S/N)");
            string? res = Console.ReadLine();

            if (res.ToUpper().Equals("S"))
            {
                Console.Clear();
                Console.WriteLine(texto);

                System.IO.File.Copy(dbOriginal, dbEditado2, true);
                ManipulaBanco db = new ManipulaBanco(dbEditado2);
                db.connection.Open();

                Console.WriteLine("\n\n[1]  Adicionar um novo banco de dados  (É necessário utilizar esse processo quando for a primeira biometria migrada do condomínio)");
                Console.WriteLine("[2]  Manter o banco antigo e adicionar novos dados (É necessário utilizar esse procedimento quando for a segunda ou mais biometria a ser migrada no condomínio)");
                res = Console.ReadLine();

                if (res.Equals("1"))
                {
                    db.filtrarTabela("user_fingerprint");
                    db.atualizarUserFingerprint(idsDictionary, dadosAPI.ListUserSuccess);
                }
                else if (res.Equals("2"))
                {
                    db.filtrarTabela("user_fingerprint");
                    db.adicionarUserFingerprint(dadosAPI.ListUserSuccess);
                    
                }

                db.connection.Close();

                File.Copy(dbEditado2, dbEditado, true);
                try
                {
                    client.enviarBancoDeDados(dbEditado);
                    Console.WriteLine("Banco da CPU atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao atualizar o banco da CPU");
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Banco da CPU não atualizado!");
            }

            client.desconectar();

            if (dadosAPI.ListUserFailed.Count == 0)
            {
                Console.WriteLine("---------------------------------------- INTEGRAÇÃO COM SUCESSO! ------------------------------------------------");
            }

            Console.WriteLine("Finalizado!!!\n");
            Console.ReadKey();
        }
    }
}