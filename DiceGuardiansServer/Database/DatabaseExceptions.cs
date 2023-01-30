namespace DiceGuardiansServer.Database;

public class EntryDoesNotExistException : Exception {
    public EntryDoesNotExistException() {}
    public EntryDoesNotExistException(string message) : base(message) {}
}

public class InvalidDeckSizeException : Exception {
    public InvalidDeckSizeException() {}
    public InvalidDeckSizeException(string message) : base(message) {}
}