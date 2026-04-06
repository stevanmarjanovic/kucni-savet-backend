namespace KucniSavetBackend.Exceptions;


public class NotFoundException(string message) : Exception(message);

public class NotFoundException<T>(string key) : NotFoundException($"{typeof(T).Name} {key} not found.");

