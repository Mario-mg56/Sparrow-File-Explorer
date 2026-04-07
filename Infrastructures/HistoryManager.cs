using System;
using System.Collections.Generic;
using System.Linq;
using DynamicFileExplorer;
using DynamicFileExplorer.Infrastructures;

public class HistoryManager()
{
    FileManager fm = App.Current.fileManager;

    private Stack<HistorialAction> lastDirectory = [];
    private Stack<HistorialAction> nextDirectory = [];
    private HistorialAction? accionEjecutada;
    public void GoForward()
    {
        if(accionEjecutada==null)return;
        if(lastDirectory.Count()==0)return;
        HistorialAction ultimaAccion = lastDirectory.Pop();
        ultimaAccion.Invoke();
        nextDirectory.Push(accionEjecutada);
        accionEjecutada = ultimaAccion;
        Console.WriteLine("acciones restasntes" + lastDirectory.Count());
        // foreach (var item in lastDirectory)
        // {
        //     Console.WriteLine(item);
        // }
    }
    public void GoBackward()
    {
        if(accionEjecutada==null)return;
        if(nextDirectory.Count()==0)return;
        HistorialAction ultimaAccion = nextDirectory.Pop();
        ultimaAccion.Invoke();
        lastDirectory.Push(accionEjecutada);
        accionEjecutada = ultimaAccion;
    }


    public void stackHaciaAlante(Action accion, String message)
    {
        nextDirectory.Clear();
        if(accionEjecutada!=null)
            lastDirectory.Push(accionEjecutada);
        accionEjecutada = new HistorialAction(accion,message);
        Console.WriteLine(accionEjecutada);
    }
}