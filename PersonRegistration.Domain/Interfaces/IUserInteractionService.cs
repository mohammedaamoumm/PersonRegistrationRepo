namespace PersonRegistration.Domain.Interfaces
{
    public interface IUserInteractionService
    {
        string GetInput(string prompt);
        void ShowMessage(string message);
    }
}
