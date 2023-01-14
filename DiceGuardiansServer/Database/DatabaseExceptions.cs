namespace DiceGuardiansServer.Database;

public class EntryDoesNotExistException : Exception {
    public EntryDoesNotExistException() {}

    public EntryDoesNotExistException(string message) : base(message) {}
}