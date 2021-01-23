using System.Collections.Generic;
using System.Linq;
using VsChromium.Core.Ipc.TypedMessages;
using VsChromium.Core.Files;

namespace VsChromium.Features.ToolWindows.OpenFile {
  public class FileEntryViewModel {
    private DirectoryEntry _dirEntry;
    private FileEntry _fileEntry;
    private string _path;
    private string _filename;

    public FileEntryViewModel(DirectoryEntry dirEntry, FileEntry fileEntry) {
      _dirEntry = dirEntry;
      _fileEntry = fileEntry;

      _path = PathHelpers.CombinePaths(dirEntry.Name, fileEntry.Name);
      _filename = PathHelpers.GetFileName(_path);
    }

    static public IEnumerable<FileEntryViewModel> Create(FileSystemEntry systemEntry) {
      FileEntry fileEntry = systemEntry as FileEntry;
      if (fileEntry != null) {
        return new[] { new FileEntryViewModel(null, fileEntry) };
      } else {
        DirectoryEntry dirEntry = systemEntry as DirectoryEntry;
        if (dirEntry != null) {
          return dirEntry
            .Entries
            .Select(entry => new FileEntryViewModel(dirEntry, (FileEntry)entry))
            .ToList();
        }
      }

      return new List<FileEntryViewModel>();
    }

    public string Filename {
      get { return _filename; }
    }

    public string Path {
      get { return _path; }
    }
  }
}
