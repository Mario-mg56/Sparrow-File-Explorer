using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DynamicFileExplorer.Models;

namespace DynamicFileExplorer.Infrastructures;
public class SearchManager(FileManager fm)
{
    public List<FileSystemItem> searchWorkingDir{get;} = [];
    public IReadOnlyList<FileSystemItem> GetResults() => searchWorkingDir;
    public FileManager fm = fm;
    public void CleanSearch()
    {
        fm.Execute(() =>
        {
            searchWorkingDir.Clear();
            fm.CastWorkingDirChanged();
        }, "limpiando historial fe la ultima busqueda");
        
    }

    public void Clear()
    {
        searchWorkingDir.Clear();
    }
    public bool IsEmpty()
    {
        return searchWorkingDir.Count()==0;
    }
    public async Task SearchWorkingDir(string word)
    {
        List<FileSystemItem> resultados = [];
       
        var stack = new Stack<string>();
        stack.Push(fm.WorkingDir.GetPath());
        while (stack.Count > 0)
        {
            var dir = stack.Pop();

            try
            {
                foreach (var file in Directory.EnumerateFiles(dir))
                {
                    if (System.IO.Path.GetFileName(file).Contains(word))
                        {
                        resultados.Add(new Models.File(new Models.Path(file)));
                    }
                }

                foreach (var subDir in Directory.EnumerateDirectories(dir))
                {
                    if (System.IO.Path.GetFileName(subDir).Contains(word))
                    {
                        resultados.Add(new DirItem(new Models.Path(subDir)));
                    }

                    stack.Push(subDir);
                }
            }
            catch { }
        }
        Console.WriteLine("Se ha buscado la palabra"+ word + "resultados: " +resultados.Count());
        fm.Execute(()=>ActualizeSearchWorkingDirList(resultados), "cambiando el directorio a una busqueda ");
}

    public void ActualizeSearchWorkingDirList(List<FileSystemItem> lista)
    {
        searchWorkingDir.Clear();
        lista.ForEach(searchWorkingDir.Add);
        fm.CastWorkingDirChanged();
    }
}