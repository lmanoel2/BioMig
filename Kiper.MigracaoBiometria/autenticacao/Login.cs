using System.Text;


namespace Kiper.MigracaoBiometria.Autenticacao
{
    public class Login
    {
        public string LerSenha()
        {
            StringBuilder pw = new StringBuilder();
            bool caracterApagado = false;

            while (true)
            {
                ConsoleKeyInfo cki = Console.ReadKey(true);

                if (cki.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (deletarTexto(cki))
                {
                    if (pw.Length != 0)
                    {
                        Console.Write("\b \b");
                        pw.Length--;

                        caracterApagado = true;
                    }
                }
                else
                {
                    caracterApagado = false;
                }

                if (!caracterApagado && verificarCaracterValido(cki))
                {
                    Console.Write("*");
                    pw.Append(cki.KeyChar);
                }
            }

            return pw.ToString();
        }

        private bool verificarCaracterValido(ConsoleKeyInfo tecla)
        {
            if (char.IsLetterOrDigit(tecla.KeyChar) || char.IsPunctuation(tecla.KeyChar) ||
                char.IsSymbol(tecla.KeyChar))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private bool deletarTexto(ConsoleKeyInfo tecla)
        {
            if (tecla.Key == ConsoleKey.Backspace || tecla.Key == ConsoleKey.Delete)
                return true;
            else
                return false;
        }
    }
}
