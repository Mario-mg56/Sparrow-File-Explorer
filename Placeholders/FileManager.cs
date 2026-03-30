using System;
using System.Collections.Generic;

namespace DynamicFileExplorer.Placeholders;

class FileManager {
    public readonly List<FilePlaceholder> files;

    public FileManager(int filesNum) {
        files = [];
        for (int i = 1; i<=filesNum; i++) 
            files.Add(new FilePlaceholder("File " + i));
    }
    
}