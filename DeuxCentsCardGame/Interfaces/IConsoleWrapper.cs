namespace DeuxCentsCardGame.Interfaces
{
    public interface IConsoleWrapper
    {
        void Clear();
        void WriteLine(string message);
        void WriteLine(string format, params object[] args);
        string ReadLine();
        ConsoleKeyInfo ReadKey();
        void SetCursorPosition(int left, int top);
        int WindowWidth { get; }
        int WindowHeight { get; }
    }
}