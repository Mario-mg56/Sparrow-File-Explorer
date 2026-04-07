
using System;

public class HistorialAction(Action accion, String message) 
{
    Action accion = accion;
    string message = message;

    public void Invoke()
    {
        accion.Invoke();
        Console.WriteLine("Realizando " + message);
    }

    public override string ToString()
    {
        return message;
    }
}