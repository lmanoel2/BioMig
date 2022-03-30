using System;
using Microsoft.Data.Sqlite;

namespace banco_de_dados
{
    class ManipulaBanco
    {
        public SqliteConnection connection;
        private SqliteCommand command;

        public ManipulaBanco(string nome)
        {
            connection = new SqliteConnection($"Data Source={nome}");
            command = connection.CreateCommand();
        }

        public void filtrarTabela(string identificacao)
        {
            try
            {
                command.CommandText = @$"SELECT * FROM {identificacao}";
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void atualizarFpTerminal(List<long> user_id)
        {
            try
            {
                user_id.Sort();
                string joinedUser_id = String.Join(", ", user_id.ToArray());
                command.CommandText = "   UPDATE fp_terminal SET user_list = '{"+joinedUser_id+"}' WHERE leader = 1;";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Não foi possível atualizar a tabela Fp_terminal, ocorreu um erro.");
                Console.WriteLine(ex.Message);
            }
            command.ExecuteNonQueryAsync();

        }
        public void atualizarUserFingerprint(Dictionary<long, long>? idsDictionary, List<long> user_id)
        {
            //(SIGMA, MONITORING)

            command.CommandText = "DELETE FROM user_fingerprint WHERE fingerprint<20";
            command.ExecuteNonQueryAsync();

            command.CommandText = "SELECT * FROM user_fingerprint";
            command.ExecuteNonQuery();

            foreach (long idMonitoring in user_id)
            {
                command.CommandText = $"INSERT INTO user_fingerprint VALUES ({idMonitoring}, 2);";
                command.ExecuteNonQueryAsync();
            }

            //long idSigma = 0;
            //foreach (long idMonitoring in user_id)
            //{
            //    foreach (var idDictionary in idsDictionary)
            //    {
            //        if (idDictionary.Value == idMonitoring)
            //        {
            //            idSigma = idDictionary.Key;
            //        }
            //    }

            //    try
            //    {
            //        command.CommandText = $"UPDATE user_fingerprint SET user_id={idMonitoring} WHERE user_id={idSigma}";
            //        command.ExecuteNonQueryAsync();
            //    }
            //    catch
            //    {
            //        Console.WriteLine($"Usuários {idSigma} não encontrado na tabela");
            //    }

            //}
        }

        public void adicionarUserFingerprint(List<long> user_id)
        {
            foreach (long idMonitoring in user_id)
            {
                command.CommandText = $"INSERT INTO user_fingerprint VALUES ({idMonitoring}, 2);";
                command.ExecuteNonQueryAsync();
            }
        }
        public void adicionarFpTerminal(List<long> user_existentes, List<long> user_id)
        {
            try
            {
                foreach (long user in user_id)
                {
                    user_existentes.Add(user);
                }
                user_existentes.Sort();
                string joinedUser_id = String.Join(", ", user_existentes.ToArray());
                command.CommandText = "   UPDATE fp_terminal SET user_list = '{" + joinedUser_id + "}' WHERE leader = 1;";
                command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Não foi possível atualizar a tabela Fp_terminal, ocorreu um erro.");
                Console.WriteLine(ex.Message);
            }
            
        }

        public List<long> pegarListaSincronizacaoBiometria()
        {
            List<long> listValores = new List<long>();
            try
            {
                command.CommandText = "SELECT user_list FROM fp_terminal WHERE leader = 1";
                command.ExecuteNonQueryAsync();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string valores = reader.GetString(0);
                        valores = valores.Replace("{", string.Empty);
                        valores = valores.Replace("}", string.Empty);
                        listValores = Array.ConvertAll(valores.Split(','), s => long.Parse(s)).ToList();
                    }
                }
                return listValores;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro: ");
                Console.WriteLine(ex.Message);
            }
            return null;


        }

        public void lerBanco()
        {
            try
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetString(0);
                        var name2 = reader.GetString(1);

                        Console.WriteLine($"Hello, {name}  {name2}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: ");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
