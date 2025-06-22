namespace DeuxCentsCardGame
{
    public class ConsoleWrapper : IConsoleWrapper
    {
        public void Clear() => Console.Clear();
        public void WriteLine(string message) => Console.WriteLine(message);
        public void WriteLine(string format, params object[] args) => Console.WriteLine(format, args);
        public string ReadLine() => Console.ReadLine() ?? string.Empty;
        public ConsoleKeyInfo ReadKey() => Console.ReadKey();
        public void SetCursorPosition(int left, int top) => Console.SetCursorPosition(left, top);
        public int WindowWidth => Console.WindowWidth;
        public int WindowHeight => Console.WindowHeight;
    }
}