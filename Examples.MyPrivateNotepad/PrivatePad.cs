using DotWrapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using DotWrapper.Resolve;

namespace MyPrivateNotepad
{
    internal class PrivatePad
    {
        private readonly Wrap _wrap;

        public IEnumerable<TabModel> ClosedTabs
        {
            get
            {
                return _wrap.Chunks
                    .Select((c, i) => new TabModel(i, c.Name, Encoding.Default.GetString(c.Data)))
                    .Where((c, i) => OpenTabs.All(t => t.Pos != i));
            }
        }

        public string Path { get; set; }

        /// <summary>
        ///     Gets or sets the chunks (which correspond to tabs).
        /// </summary>
        public ObservableCollection<TabModel> OpenTabs { get; set; }

        public PrivatePad(string path)
        {
            //Wrap.Salt = Encoding.Default.GetBytes("$P€çi@L_SA17");
            Path = path;
            OpenTabs = new ObservableCollection<TabModel>();
            try
            {
                _wrap = Wrap.Read(path);
            }
            catch (Exception)
            {
                const string header = "----\n" +
                                      "This is a data file for MyPrivateNotepad.\n" +
                                      "Please do not edit anything here!\n" +
                                      "----\n";
                _wrap = new Wrap(Encoding.Default.GetBytes(header));
            }
        }

        /// <summary>
        ///     Returns true if save succeeded.
        ///     False two tabs have the same name.
        /// </summary>
        public bool Save()
        {
            if (!CheckNames())
                return false;
            foreach (TabModel t in OpenTabs)
                _wrap.Chunks[t.Pos].Data = Encoding.Default.GetBytes(t.Content);
            _wrap.Write(Path);
            return true;
        }

        public bool CheckNames()
        {
            if (!OpenTabs.Any(t => CheckTabName(t.Pos) > 1))
                return true;
            const string content = "You cannot save when multiple tabs have the same name.\n" +
                                   "Please fix this before.";
            const string title = "Saving Error";
            MessageBox.Show(content, title, MessageBoxButton.OK);
            return false;
        }

        #region Tabs Operations

        public TabModel OpenTab(string name, string password)
        {
            Chunk tabChunk = _wrap.Chunks.Find(c => c.Name == name);
            // Create chunk if it does not exist.
            if (tabChunk == null)
            {
                tabChunk = new Chunk(name, password);
                _wrap.Chunks.Add(tabChunk);
            }
            string txt = Encoding.Default.GetString(tabChunk.Data);
            TabModel model = new TabModel(_wrap.Chunks.IndexOf(tabChunk), tabChunk.Name, txt);
            OpenTabs.Add(model);
            return model;
        }

        public bool CloseTab(string name)
        {
            if (!CheckNames())
                return false;
            TabModel chunk = GetTab(name);
            return (chunk != null && OpenTabs.Remove(chunk));
        }
        public bool CloseAllTabs()
        {
            if (!CheckNames())
                return false;
            OpenTabs.Clear();
            return true;
        }

        private TabModel GetTab(string name)
        {
            return OpenTabs.FirstOrDefault(tabModel => tabModel.Name == name);
        }

        public bool TabTitleChanged(int pos, string newValue)
        {
            OpenTabs.First(t => t.Pos == pos).Name = newValue;
            // Change the name of the chunk for name checking.
            Chunk chunk = _wrap.Chunks[pos];
            chunk.Name = newValue;
            return (CheckTabName(pos) == 1);
        }

        public void TabContentChanged(int pos, string newValue)
        {
            OpenTabs.First(t => t.Pos == pos).Content = newValue;
        }

        /// <summary>
        ///     Counts the notes that have the same name.
        /// </summary>
        /// <param name="pos">the position of the chunk to compare names with</param>
        /// <returns>the number of notes found</returns>
        public int CheckTabName(int pos)
        {
            string name = _wrap.Chunks[pos].Name;
            return CheckTabName(name);
        }

        /// <summary>
        ///     Counts the notes that have the same name.
        /// </summary>
        /// <param name="name">the name to compare</param>
        /// <returns>the number of notes found</returns>
        public int CheckTabName(string name)
        {
            return _wrap.Chunks.Count(c => c.Name == name);
        }

        public bool CheckPassword(string name, string password)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            Chunk chunk = _wrap.Chunks.Find(c => c.Name == name);
            return (((CryptoResolver)chunk.ResolveChain).Password == password);
        }

        #endregion Tabs Operations

    }
}
